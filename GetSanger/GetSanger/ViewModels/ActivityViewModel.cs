using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(ConnectedActivity), "activity")]
    public class ActivityViewModel : BaseViewModel
    {
        #region Fields
        private Activity m_ConnectedActivity;
        private string m_Location;
        private string m_JobLocation;
        private string m_Phone;
        private string m_ActivatedButtonText;
        private bool m_IsActivetdLocationButton;
        private bool m_IsActivatedEndButton;
        #endregion

        #region Properties
        public Activity ConnectedActivity
        {
            get => m_ConnectedActivity;
            set => SetClassProperty(ref m_ConnectedActivity, value);
        }

        public bool IsActivatedEndButton
        {
            get => m_IsActivatedEndButton;
            set => SetStructProperty(ref m_IsActivatedEndButton, value);
        }

        public bool IsActivatedLocationButton
        {
            get => m_IsActivetdLocationButton;
            set => SetStructProperty(ref m_IsActivetdLocationButton, value);
        }

        public string ActivatedButtonText
        {
            get => m_ActivatedButtonText;
            set => SetClassProperty(ref m_ActivatedButtonText, value);
        }

        public string Location
        {
            get => m_Location; 
            set => SetClassProperty(ref m_Location, value); 
        }

        public string JobLocation
        {
            get => m_JobLocation; 
            set => SetClassProperty(ref m_JobLocation, value); 
        }

        public string Phone
        {
            get => m_Phone;
            set => SetClassProperty(ref m_Phone, value);
        }

        public string Category { get => ConnectedActivity.JobDetails.Category.ToString(); }
        #endregion

        #region Commands

        public ICommand ProfileCommand { get; private set; }

        public ICommand LocationCommand { get; private set; }

        public ICommand EndActivityCommand { get; private set; }

        #endregion

        #region Constructor
        public ActivityViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            setLocationsLabels();
            IsActivatedLocationButton = ConnectedActivity.Status.Equals(ActivityStatus.Active);
            IsActivatedEndButton = AppManager.Instance.ConnectedUser.UserId.Equals(ConnectedActivity.SangerID) &&
                                   AppManager.Instance.CurrentMode.Equals(AppMode.Sanger) &&
                                   ConnectedActivity.Status.Equals(ActivityStatus.Active) == true;
        }

        private void setCommands()
        {
            ProfileCommand = new Command(profilePage);
            EndActivityCommand = new Command(endActivity);
            LocationCommand = new Command(locationCommandHelper);
        }

        private async void setLocationsLabels()
        {
            Location = await getLocationString(ConnectedActivity.JobDetails.Location);
            JobLocation = await getLocationString(ConnectedActivity.JobDetails.JobLocation);
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Client))
            {
                ActivatedButtonText = string.Format($"{(ConnectedActivity.LocationActivatedBySanger ? "See" : "Ask the sanger to active")} Location");
            }
            else if (AppManager.Instance.CurrentMode.Equals(AppMode.Sanger))
            {
                ActivatedButtonText = string.Format($"{(ConnectedActivity.LocationActivatedBySanger == false ? "Enable" : "Disable")} Location");
            }

        }

        private void locationCommandHelper()
        {
            AppMode mode = AppManager.Instance.CurrentMode;
            switch (mode)
            {
                case AppMode.Sanger:
                    doSanger();
                    break;
                case AppMode.Client:
                    doUser();
                    break;
                default:
                    throw new ArgumentException("Could not find the mode of the client!");
            }
        }

        private async void doSanger()
        {
            User user = await RunTaskWhileLoading(FireStoreHelper.GetUser(ConnectedActivity.ClientID));
            bool sangerInuser = user.ActivatedMap.TryGetValue(ConnectedActivity.SangerID, out bool activated);
            if (sangerInuser && activated)
            {
                // sanger ends location
                bool agreed = await r_PageService.DisplayAlert("Note", $"Do you want to stop sharing your location with {user.PersonalDetails.NickName}?", "OK", "cancel");
                if (agreed)
                {
                    user.ActivatedMap.Add(ConnectedActivity.ActivityId, false);
                    ConnectedActivity.LocationActivatedBySanger = false;
                    await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(ConnectedActivity));
                    await RunTaskWhileLoading(FireStoreHelper.UpdateUser(user));
                    r_LocationServices.LeaveTripThread();
                    await RunTaskWhileLoading(r_PushService.SendToDevice<string>(user.UserId, null, null, "Location sharing stopped", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} stopped sharing the location with you!"));
                    ActivatedButtonText = "Enable Location";
                }
            }
            else
            {
                // sanger starts location
                // sanger always write his location to DB - on start the application
                bool agreed = await r_PageService.DisplayAlert("Note", $"Do you want to share your location with {user.PersonalDetails.NickName}?", "OK", "cancel");
                if (agreed)
                {
                    user.ActivatedMap.Add(ConnectedActivity.ActivityId, true);
                    ConnectedActivity.LocationActivatedBySanger = true;
                    await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(ConnectedActivity));
                    await RunTaskWhileLoading(FireStoreHelper.UpdateUser(user));
                    await RunTaskWhileLoading(r_PushService.SendToDevice<string>(user.UserId, null, null, "Location sharing allowed", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} shared the location with you!"));
                    r_LocationServices.StartTripThread();
                    ActivatedButtonText = "Disable Location";
                }
            }
        }

        private async void doUser()
        {
            User user = await RunTaskWhileLoading(FireStoreHelper.GetUser(ConnectedActivity.ClientID));
            bool sangerInUser = user.ActivatedMap.TryGetValue(ConnectedActivity.ActivityId, out bool activated);
            if (sangerInUser && activated)
            {
                await r_NavigationService.NavigateTo(ShellRoutes.Map + $"?connectedpage={this}");
            }
            else
            {
                User sanger = await RunTaskWhileLoading(FireStoreHelper.GetUser(ConnectedActivity.SangerID));
                await r_PageService.DisplayAlert("Note", $"{sanger.PersonalDetails.NickName} did not share with you location, please contact him to share with you the location!", "OK");
            }
        }

        private async Task<string> getLocationString(Location i_Location)
        {
            Placemark placemark = await r_LocationServices.PickedLocation(i_Location);
            if (placemark == null)
            {
                return "No Location was given";
            }

            return string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare);
        }

        private async void endActivity()
        {
            IsActivatedLocationButton = !(await r_PageService.DisplayAlert("Note", "Are you sure?", "Yes", "No"));
            if(IsActivatedLocationButton == false)
            {
                ConnectedActivity.Status = ActivityStatus.Completed;
                ConnectedActivity.LocationActivatedBySanger = false;
                r_LocationServices.LeaveTripThread(); // sanger stop sharing location
                await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(ConnectedActivity));
                string message = $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} completed your job :)";
                await RunTaskWhileLoading(r_PushService.SendToDevice(ConnectedActivity.ClientID, ConnectedActivity, ConnectedActivity.GetType(), "Job has ended", message));
                IsActivatedEndButton = false;
                await GoBack();
            }
        }

        private async void profilePage()
        {
            User user = ConnectedActivity.ClientID.Equals(AppManager.Instance.ConnectedUser.UserId) ?
                        await RunTaskWhileLoading(FireStoreHelper.GetUser(ConnectedActivity.SangerID)) :
                        await RunTaskWhileLoading(FireStoreHelper.GetUser(ConnectedActivity.ClientID));

            await r_NavigationService.NavigateTo(ShellRoutes.Profile + $"?user={user}");
        }

        #endregion
    }
}
