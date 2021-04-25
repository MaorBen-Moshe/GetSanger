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
        private string m_Location;
        private string m_JobLocation;
        private string m_Phone;
        private string m_ActivatedButtonText;
        private bool m_IsActivetdLocationButton;
        private bool m_IsActivatedEndButton;
        #endregion

        #region Properties
        public Activity ConnectedActivity { get; set; }

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
            get { return m_Location; }
            set { SetClassProperty(ref m_Location, value); }
        }

        public string JobLocation
        {
            get { return m_JobLocation; }
            set { SetClassProperty(ref m_JobLocation, value); }
        }

        public string Phone
        {
            get => m_Phone;
            set => SetClassProperty(ref m_Phone, value);
        }

        public string Category { get => ConnectedActivity.JobDetails.Category.ToString(); }
        public DateTime Date { get => ConnectedActivity.JobDetails.Date; }
        public string Description { get => ConnectedActivity.JobDetails.Description; }
        #endregion

        #region Commands

        public ICommand ProfileCommand { get; private set; }

        public ICommand LocationCommand { get; private set; }

        public ICommand EndActivityCommand { get; private set; }

        #endregion

        #region Constructor
        public ActivityViewModel()
        {
            ProfileCommand = new Command(profilePage);
            EndActivityCommand = new Command(endActivity);
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            setLocationsLabels();
            initialPhoneNumber();
            IsActivatedLocationButton = true;
            IsActivatedEndButton = AppManager.Instance.ConnectedUser.UserID.Equals(ConnectedActivity.SangerID) &&
                                   AppManager.Instance.CurrentMode.Equals(AppMode.Sanger) &&
                                   ConnectedActivity.Status.Equals(ActivityStatus.Completed) == false;
        }

        private async void setLocationsLabels()
        {
            Location = await getLocationString(ConnectedActivity.JobDetails.Location);
            JobLocation = await getLocationString(ConnectedActivity.JobDetails.JobLocation);
            LocationCommand = new Command(locationCommandHelper);
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
            User user = await FireStoreHelper.GetUser(ConnectedActivity.ClientID);
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
                    LocationServices.LeaveTripThread();
                    r_PushService.SendToDevice<string>(user.UserID, null, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} stopped sharing the location with you!");
                    ActivatedButtonText = "Enable Location";
                }
            }
            else
            {
                // sanger starts location
                bool agreed = await r_PageService.DisplayAlert("Note", $"Do you want to share your location with {user.PersonalDetails.NickName}?", "OK", "cancel");
                if (agreed)
                {
                    user.ActivatedMap.Add(ConnectedActivity.ActivityId, true);
                    ConnectedActivity.LocationActivatedBySanger = true;
                    await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(ConnectedActivity));
                    await RunTaskWhileLoading(FireStoreHelper.UpdateUser(user));
                    r_PushService.SendToDevice<string>(user.UserID, null, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} shared the location with you!");
                    LocationServices.StartTripThread();
                    ActivatedButtonText = "Disable Location";
                }
            }
        }

        private async void doUser()
        {
            User user = await FireStoreHelper.GetUser(ConnectedActivity.ClientID);
            bool sangerInUser = user.ActivatedMap.TryGetValue(ConnectedActivity.ActivityId, out bool activated);
            if (sangerInUser && activated)
            {
                await Shell.Current.GoToAsync($"/map?connectedpage={this}");
            }
            else
            {
                User sanger = await FireStoreHelper.GetUser(ConnectedActivity.SangerID);
                await r_PageService.DisplayAlert("Note", $"{sanger.PersonalDetails.NickName} did not share with you location, please contact him to share with you the location!", "OK");
            }
        }

        private async void initialPhoneNumber()
        {
            AppMode mode = AppManager.Instance.CurrentMode;
            User user;
            switch (mode)
            {
                case AppMode.Client:
                    user = await FireStoreHelper.GetUser(ConnectedActivity.SangerID);
                    Phone = user.PersonalDetails.Phone.PhoneNumber;
                    break;
                case AppMode.Sanger:
                    user = await FireStoreHelper.GetUser(ConnectedActivity.ClientID);
                    Phone = user.PersonalDetails.Phone.PhoneNumber;
                    break;
                default:
                    throw new ArgumentException("No Phone were in DB");
            }
        }

        private async Task<string> getLocationString(Location i_Location)
        {
            Placemark placemark = await LocationServices.PickedLocation(i_Location);
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
                LocationServices.LeaveTripThread(); // sanger stop sharing location
                await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(ConnectedActivity));
                IsActivatedEndButton = false;
            }
        }

        private async void profilePage()
        {
            User user = ConnectedActivity.ClientID.Equals(AppManager.Instance.ConnectedUser.UserID) ?
                        await FireStoreHelper.GetUser(ConnectedActivity.SangerID) :
                        await FireStoreHelper.GetUser(ConnectedActivity.ClientID);

            await Shell.Current.GoToAsync($"/profile?user={user}");
        }

        #endregion
    }
}
