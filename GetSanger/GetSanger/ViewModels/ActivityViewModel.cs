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
        private string m_Location;
        private string m_JobLocation;
        private string m_Phone;
        private string m_ActivatedButtonText;
        public Activity ConnectedActivity { get; set; }

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

        public string Category { get => ConnectedActivity.JobOffer.Category.ToString(); }
        public DateTime Date { get => ConnectedActivity.JobOffer.Date; }
        public string Description { get => ConnectedActivity.JobOffer.Description; }

        public ICommand ProfileCommand { get; private set; }
        public ICommand LocationCommand { get; private set; }

        public ActivityViewModel()
        {
            setLocationsLabels();
            initialPhoneNumber();
        }

        private async void setLocationsLabels()
        {
            Location = await getLocation(ConnectedActivity.JobOffer.Location);
            JobLocation = await getLocation(ConnectedActivity.JobOffer.JobLocation);
            LocationCommand = new Command(locationCommandHelper);
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Client))
            {
                ActivatedButtonText = string.Format($"{(ConnectedActivity.LocationActivatedBySanger ? "See" : "Ask from sanger to active")} Location");
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
            if(sangerInuser && activated)
            {
                bool agreed = await r_PageService.DisplayAlert("Note", $"Do you want to stop sharing your location with {user.PersonalDetails.Nickname}?", "ok", "cancel");
                if (agreed)
                {
                    user.ActivatedMap.Add(ConnectedActivity.ActivityId, false);
                    AppManager.Instance.ConnectedUser.UserLocation = await LocationServices.GetCurrentLocation();
                    FireStoreHelper.UpdateUser(user);
                    FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                    LocationServices.LeaveTripThread(handleSangerLocation);
                    // we can send push to user to tell that if he is in the map 
                }

                ActivatedButtonText = "Enable Location";
            }
            else
            {
                bool agreed = await r_PageService.DisplayAlert("Note", $"Do you want to share your location with {user.PersonalDetails.Nickname}?", "ok", "cancel");
                if (agreed)
                {
                    user.ActivatedMap.Add(ConnectedActivity.ActivityId, true);
                    FireStoreHelper.UpdateUser(user);
                    LocationServices.HandleTripThread(handleSangerLocation);
                }

                ActivatedButtonText = "Disable Location";
            }
        }

        private async void doUser()
        {
            User user = await FireStoreHelper.GetUser(ConnectedActivity.ClientID);
            bool sangerInUser = user.ActivatedMap.TryGetValue(ConnectedActivity.ActivityId, out bool activated);
            if(sangerInUser && activated)
            {
                await Shell.Current.GoToAsync($"/map?connectedpage={this}");
            }
            else 
            {
                User sanger = await FireStoreHelper.GetUser(ConnectedActivity.SangerID);
                await r_PageService.DisplayAlert("Note", $"{sanger.PersonalDetails.Nickname} did not share with you location, please contact him to share with you the location!", "ok", "cancel");
            }
        }

        private void handleSangerLocation(object sender, System.Timers.ElapsedEventArgs e)
        {

        }

        private async void initialPhoneNumber()
        {
            AppMode mode = AppManager.Instance.CurrentMode;
            User user;
            switch (mode)
            {
                case AppMode.Client:
                    user = await FireStoreHelper.GetUser(ConnectedActivity.ClientID);
                    Phone = user.PersonalDetails.Phone.PhoneNumber;
                    break;
                case AppMode.Sanger:
                    user = await FireStoreHelper.GetUser(ConnectedActivity.SangerID);
                    Phone = user.PersonalDetails.Phone.PhoneNumber;
                    break;
                default:
                    throw new ArgumentException("No Phone were in DB");
            }
        }

        private async Task<string> getLocation(Location i_Location)
        {
            Placemark placemark = await LocationServices.PickedLocation(i_Location);
            if(placemark == null)
            {
                return "No Location was given";
            }

            return string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare);
        }
    }
}
