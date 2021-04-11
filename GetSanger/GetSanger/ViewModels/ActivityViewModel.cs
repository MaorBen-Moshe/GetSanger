using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class ActivityViewModel : BaseViewModel
    {
        private string m_Location;
        private string m_JobLocation;
        private string m_Phone;
        private string m_ActivatedButtonText;
        public Activity ConnectedActivity { get; }

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
            ActivatedButtonText = string.Format($"{(ConnectedActivity.LocationActivatedBySanger ? "See" : "Active")} Location");
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
                    user.ActivatedMap.Add(ConnectedActivity.SangerID, false);
                    FireStoreHelper.UpdateUser(user);
                }
            }
            else
            {
                bool agreed = await r_PageService.DisplayAlert("Note", $"Do you want to share your location with {user.PersonalDetails.Nickname}?", "ok", "cancel");
                if (agreed)
                {
                    user.ActivatedMap.Add(ConnectedActivity.SangerID, true);
                    FireStoreHelper.UpdateUser(user);
                }
            }
        }

        private async void doUser()
        {
            User user = await FireStoreHelper.GetUser(ConnectedActivity.ClientID);
            bool sangerInUser = user.ActivatedMap.TryGetValue(ConnectedActivity.SangerID, out bool activated);
            if(sangerInUser && activated)
            {
                //show map
            }
            else 
            {
                User sanger = await FireStoreHelper.GetUser(ConnectedActivity.SangerID);
                await r_PageService.DisplayAlert("Note", $"{sanger.PersonalDetails.Nickname} did not share with you location, please contact him to share with you the location!", "ok", "cancel");
            }
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
