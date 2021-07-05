using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(ActivityJson), "activity")]
    public class ActivityViewModel : BaseViewModel
    {
        #region Fields
        private Activity m_ConnectedActivity;
        private string m_Location;
        private string m_JobLocation;
        private string m_ActivatedButtonText;
        private bool m_IsActivetdLocationButton;
        private bool m_IsActivatedEndButton;
        private string m_ProfileName;
        private bool m_IsSangerNotesVisible;
        #endregion

        #region Properties
        public Activity ConnectedActivity
        {
            get => m_ConnectedActivity;
            set => SetClassProperty(ref m_ConnectedActivity, value);
        }

        public string ActivityJson
        {
            set
            {
                ConnectedActivity = ObjectJsonSerializer.DeserializeForPage<Activity>(value);
            }
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

        public string ProfileName
        {
            get => m_ProfileName;
            set => SetClassProperty(ref m_ProfileName, value);
        }

        public bool IsSangerNotesVisible
        {
            get => m_IsSangerNotesVisible;
            set => SetStructProperty(ref m_IsSangerNotesVisible, value);
        }

        #endregion

        #region Commands

        public ICommand ProfileCommand { get; private set; }

        public ICommand LocationCommand { get; private set; }

        public ICommand EndActivityCommand { get; private set; }

        public ICommand NotesCommand { get; private set; }

        #endregion

        #region Constructor
        public ActivityViewModel()
        {
            CrashlyticsService crashlyticsService = (CrashlyticsService) AppManager.Instance.Services.GetService(typeof(CrashlyticsService));
            crashlyticsService.LogPageEntrance(nameof(ActivityViewModel));

            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(ActivityViewModel));
            setLocationsLabels();
            ProfileName = setProfileName();
            IsActivatedLocationButton = ConnectedActivity.Status.Equals(ActivityStatus.Active);
            IsActivatedEndButton = AppManager.Instance.ConnectedUser.UserId.Equals(ConnectedActivity.SangerID) &&
                                   AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger) &&
                                   ConnectedActivity.Status.Equals(ActivityStatus.Active) == true;
            IsSangerNotesVisible = AppManager.Instance.ConnectedUser.UserId.Equals(ConnectedActivity.ClientID) &&
                                   AppManager.Instance.CurrentMode.Equals(eAppMode.Client);
            MessagingCenter.Subscribe<MapViewModel, bool>(this, Constants.Constants.ActivatedLocationMessage, (sender, args) =>
            {
                IsActivatedLocationButton = args;
            });
        }

        public void Disappearing()
        {
        }

        private void setCommands()
        {
            ProfileCommand = new Command(profilePage);
            EndActivityCommand = new Command(endActivity);
            LocationCommand = new Command(locationCommandHelper);
            NotesCommand = new Command(showNotes);
        }

        private string setProfileName()
        {
            string name;
            if (AppManager.Instance.ConnectedUser.UserId.Equals(ConnectedActivity.SangerID))
            {
                name = ConnectedActivity.JobDetails.ClientName;
            }
            else // the client of the activity
            {
                name = ConnectedActivity.SangerName;
            }

            return name;
        }

        private async void setLocationsLabels()
        {
            Location = await getLocationString(ConnectedActivity.JobDetails.Location);
            JobLocation = await getLocationString(ConnectedActivity.JobDetails.JobLocation);
            if (AppManager.Instance.CurrentMode.Equals(eAppMode.Client))
            {
                ActivatedButtonText = string.Format($"{(ConnectedActivity.LocationActivatedBySanger ? "See" : "Ask the sanger to active")} Location");
            }
            else if (AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger))
            {
                ActivatedButtonText = string.Format($"{(ConnectedActivity.LocationActivatedBySanger == false ? "Enable" : "Disable")} Location");
            }

        }

        private void locationCommandHelper()
        {
            eAppMode mode = AppManager.Instance.CurrentMode;
            switch (mode)
            {
                case eAppMode.Sanger:
                    doSanger();
                    break;
                case eAppMode.Client:
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
                await r_PageService.DisplayAlert("Note", 
                                                 $"Do you want to stop sharing your location with {user.PersonalDetails.NickName}?",
                                                 "OK",
                                                 "cancel",
                                                 async (agreed) =>
                                                 {
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
                                                 });
            }
            else
            {
                // sanger starts location
                // sanger always write his location to DB - on start the application
                await r_PageService.DisplayAlert("Note",
                                                 $"Do you want to share your location with {user.PersonalDetails.NickName}?",
                                                 "OK",
                                                 "cancel",
                                                 async (agreed) =>
                                                 {
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
                                                 });
            }
        }

        private async void doUser()
        {
            User user = await RunTaskWhileLoading(FireStoreHelper.GetUser(ConnectedActivity.ClientID));
            bool sangerInUser = user.ActivatedMap.TryGetValue(ConnectedActivity.ActivityId, out bool activated);
            if (sangerInUser && activated)
            {
                bool locationGranted = await r_LocationServices.IsLocationGrantedAndAskFor() == PermissionStatus.Granted;
                if (locationGranted)
                {
                    await r_NavigationService.NavigateTo($"{ShellRoutes.Map}?isTrip={true}&isSearch={false}&sangerId={ConnectedActivity.SangerID}");
                }
                else
                {
                    await r_PageService.DisplayAlert("Note", "Please allow location!", "OK");
                }
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
            await r_PageService.DisplayAlert("Note",
                                             "Are you sure?",
                                             "Yes",
                                             "No",
                                             async (notActivated) => 
                                             {
                                                 IsActivatedLocationButton = !notActivated;
                                                 if (IsActivatedLocationButton == false)
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
                                             });
        }

        private async void profilePage()
        {
            string user = ConnectedActivity.ClientID.Equals(AppManager.Instance.ConnectedUser.UserId) ?
                        ConnectedActivity.SangerID :
                       ConnectedActivity.ClientID;

            await r_NavigationService.NavigateTo($"{ShellRoutes.Profile}?userid={user}");
        }

        private async void showNotes(object i_Param)
        {
            string error = "Fail showing description in activity page";
            bool succeeded = byte.TryParse((i_Param as string), out byte result);
            if (succeeded)
            {
                switch (result)
                {
                    case 0: await PopupNavigation.Instance.PushAsync(new EditorPopup(ConnectedActivity.JobDetails.Description, "Job Description")); break;
                    case 1: await PopupNavigation.Instance.PushAsync(new EditorPopup(ConnectedActivity.JobDetails.SangerNotes, "Sanger Notes")); break;
                    default: throw new ArgumentException(error);
                }
            }
            else
            {
                throw new ArgumentException(error);
            }
        }

        #endregion
    }
}
