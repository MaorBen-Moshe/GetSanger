using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Utils;
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
        private bool m_IsJobLocationVisible;
        private bool m_IsConfirmationButtonVisible;
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

        public bool IsJobLocationVisible
        {
            get => m_IsJobLocationVisible;
            set => SetStructProperty(ref m_IsJobLocationVisible, value);
        }

        public bool IsConfirmationButtonVisible
        {
            get => m_IsConfirmationButtonVisible;
            set => SetStructProperty(ref m_IsConfirmationButtonVisible, value);
        }

        #endregion

        #region Commands

        public ICommand ProfileCommand { get; private set; }

        public ICommand LocationCommand { get; private set; }

        public ICommand EndActivityCommand { get; private set; }

        public ICommand NotesCommand { get; private set; }

        public ICommand ConfirmActivityCommand { get; set; }

        public ICommand RejectActivityCommand { get; set; }

        #endregion

        #region Constructor
        public ActivityViewModel()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(ActivityViewModel));
            SetCommands();
        }
        #endregion

        #region Methods

        public async override void Appearing()
        {
            try
            {
                sr_CrashlyticsService.LogPageEntrance(nameof(ActivityViewModel));
                setLocationsLabels();
                ProfileName = setProfileName();
                IsActivatedLocationButton = ConnectedActivity.Status.Equals(eActivityStatus.Active);
                IsActivatedEndButton = AppManager.Instance.ConnectedUser.UserId.Equals(ConnectedActivity.SangerID) &&
                                       AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger) &&
                                       ConnectedActivity.Status.Equals(eActivityStatus.Active) == true;
                IsSangerNotesVisible = AppManager.Instance.ConnectedUser.UserId.Equals(ConnectedActivity.ClientID) &&
                                       AppManager.Instance.CurrentMode.Equals(eAppMode.Client);
                IsJobLocationVisible = ConnectedActivity.JobDetails.Category.Equals(eCategory.Delivery);
                IsConfirmationButtonVisible = ConnectedActivity.Status.Equals(eActivityStatus.Active) || ConnectedActivity.Status.Equals(eActivityStatus.Pending);
                MessagingCenter.Subscribe<MapViewModel, User>(this, Constants.Constants.EndActivity, async (sender, args) =>
                {
                    User sanger = args;
                    IsActivatedLocationButton = false;
                    sanger.ActivatedMap.Remove(ConnectedActivity.ActivityId);
                    ConnectedActivity.LocationActivatedBySanger = false;
                    ConnectedActivity.Status = eActivityStatus.Completed;
                    await FireStoreHelper.UpdateActivity(ConnectedActivity);
                    await FireStoreHelper.UpdateUser(sanger);
                });
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivityViewModel)}:Appearing", "Error", e.Message);
            }
        }

        public override void Disappearing()
        {
            MessagingCenter.Unsubscribe<MapViewModel>(this, Constants.Constants.EndActivity);
        }

        protected override void SetCommands()
        {
            ProfileCommand = new Command(profilePage);
            EndActivityCommand = new Command(endActivity);
            LocationCommand = new Command(locationCommandHelper);
            NotesCommand = new Command(showNotes);
            ConfirmActivityCommand = new Command(confirmActivity);
            RejectActivityCommand = new Command(rejectActivity);
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

        private async void locationCommandHelper()
        {
            try
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
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivityViewModel)}:locationCommandHelper", "Error", e.Message);
            }
        }

        private async void doSanger()
        {
            bool sangerInuser = AppManager.Instance.ConnectedUser.ActivatedMap.TryGetValue(ConnectedActivity.ActivityId, out bool activated);
            if (sangerInuser && activated)
            {
                // sanger ends location
                await sr_PageService.DisplayAlert("Note", 
                                                 $"Do you want to stop sharing your location with {ConnectedActivity.JobDetails.ClientName}?",
                                                 "OK",
                                                 "cancel",
                                                 async (agreed) =>
                                                 {
                                                     if (agreed)
                                                     {
                                                         sr_LoadingService.ShowLoadingPage();
                                                         AppManager.Instance.ConnectedUser.ActivatedMap[ConnectedActivity.ActivityId] = false;
                                                         ConnectedActivity.LocationActivatedBySanger = false;
                                                         await FireStoreHelper.UpdateActivity(ConnectedActivity);
                                                         await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                                                         sr_TripHelper.LeaveTripThread();
                                                         await sr_PushService.SendToDevice<string>(ConnectedActivity.ClientID, null, null, "Location sharing stopped", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} stopped sharing the location with you!");
                                                         ActivatedButtonText = "Enable Location";
                                                         sr_LoadingService.HideLoadingPage();
                                                     }
                                                 });
            }
            else if(activated == false)
            {
                // sanger starts location
                // sanger always write his location to DB - on start the application
                await sr_PageService.DisplayAlert("Note",
                                                 $"Do you want to share your location with {ConnectedActivity.JobDetails.ClientName}?",
                                                 "OK",
                                                 "cancel",
                                                 async (agreed) =>
                                                 {
                                                     if (agreed)
                                                     {
                                                         sr_LoadingService.ShowLoadingPage();
                                                         AppManager.Instance.ConnectedUser.ActivatedMap[ConnectedActivity.ActivityId] = true;
                                                         ConnectedActivity.LocationActivatedBySanger = true;
                                                         await FireStoreHelper.UpdateActivity(ConnectedActivity);
                                                         await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                                                         await sr_PushService.SendToDevice<string>(ConnectedActivity.ClientID, null, null, "Location sharing allowed", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} shared the location with you!");
                                                         sr_TripHelper.StartTripThread();
                                                         ActivatedButtonText = "Disable Location";
                                                         sr_LoadingService.HideLoadingPage();
                                                     }
                                                 });
            }
        }

        private async void doUser()
        {
            if (ConnectedActivity.LocationActivatedBySanger)
            {
                bool locationGranted = await sr_LocationService.IsLocationGrantedAndAskFor() == PermissionStatus.Granted;
                if (locationGranted)
                {
                    await sr_NavigationService.NavigateTo($"{ShellRoutes.Map}?isTrip={true}&isSearch={false}&sangerId={ConnectedActivity.SangerID}");
                }
                else
                {
                    await sr_PageService.DisplayAlert("Note", "Please allow location!", "OK");
                }
            }
            else
            {
                await sr_PushService.SendToDevice(ConnectedActivity.SangerID, ConnectedActivity, typeof(Activity).Name, "Location request", $"{ConnectedActivity.JobDetails.ClientName}, asked you to activate location");
                await sr_PageService.DisplayAlert("Note", "Location request has been sent!", "OK");
            }
        }

        private async Task<string> getLocationString(Location i_Location)
        {
            Placemark placemark = await sr_LocationService.GetPickedLocation(i_Location);
            if (placemark == null)
            {
                return "No Location was given";
            }

            return string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare);
        }

        private void confirmActivity(object i_Param)
        {
            ActivitiesConfirmationHelper.ConfirmActivity(ConnectedActivity, () =>
            {
                sr_LoadingService.ShowLoadingPage();
                Appearing();
                sr_LoadingService.HideLoadingPage();
            });
        }

        private void rejectActivity(object i_Param)
        {
            ActivitiesConfirmationHelper.RejectActivity(ConnectedActivity, () =>
            {
                sr_LoadingService.ShowLoadingPage();
                Appearing();
                sr_LoadingService.HideLoadingPage();
            });
        }

        private async void endActivity()
        {
            await sr_PageService.DisplayAlert("Note",
                                             "Are you sure?",
                                             "Yes",
                                             "No",
                                             async (notActivated) => 
                                             {
                                                 IsActivatedLocationButton = !notActivated;
                                                 if (IsActivatedLocationButton == false)
                                                 {
                                                     ConnectedActivity.Status = eActivityStatus.Completed;
                                                     AppManager.Instance.ConnectedUser.ActivatedMap.Remove(ConnectedActivity.ActivityId);
                                                     ConnectedActivity.LocationActivatedBySanger = false;
                                                     sr_TripHelper.LeaveTripThread(); // sanger stop sharing location
                                                     await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(ConnectedActivity));
                                                     string message = $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} completed your job :)";
                                                     await RunTaskWhileLoading(sr_PushService.SendToDevice(ConnectedActivity.ClientID, ConnectedActivity, typeof(Activity).Name, "Job has ended", message));
                                                     IsActivatedEndButton = false;
                                                     await GoBack();
                                                 }
                                             });
        }

        private async void profilePage()
        {
            try
            {
                string user = ConnectedActivity.ClientID.Equals(AppManager.Instance.ConnectedUser.UserId) ?
                            ConnectedActivity.SangerID :
                           ConnectedActivity.ClientID;

                await sr_NavigationService.NavigateTo($"{ShellRoutes.Profile}?userid={user}");
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivityViewModel)}:profilePage", "Error", e.Message);
            }
        }

        private async void showNotes(object i_Param)
        {
            try
            {
                string error = "Fail showing description in activity page";
                bool succeeded = byte.TryParse((i_Param as string), out byte result);
                if (succeeded)
                {
                    switch (result)
                    {
                        case 0: await PopupNavigation.Instance.PushAsync(new EditorPopup(ConnectedActivity.JobDetails.Description, "Job Description")); break;
                        case 1: await PopupNavigation.Instance.PushAsync(new EditorPopup(ConnectedActivity.SangerNotes, "Sanger Notes")); break;
                        default: throw new ArgumentException(error);
                    }
                }
                else
                {
                    throw new ArgumentException(error);
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivityViewModel)}:showNotes", "Error", e.Message);
            }
        }

        #endregion
    }
}