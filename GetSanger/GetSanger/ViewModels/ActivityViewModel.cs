using GetSanger.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace GetSanger.ViewModels
{
    public class ActivityViewModel : BaseViewModel
    {
        private string m_Location;
        private string m_JobLocation;
        public Activity ConnectedActivity { get; }

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
        public string Phone { get; /*get sanger or client phone*/ }
        public string Category { get => ConnectedActivity.JobOffer.Category; }
        public string SubCategory { get => ConnectedActivity.JobOffer.SubCategory; }
        public DateTime Date { get => ConnectedActivity.JobOffer.Date; }
        public string Description { get => ConnectedActivity.JobOffer.Description; }


        public ICommand ProfileCommand { get; private set; }
        public ICommand LocationCommand { get; private set; }

        public ActivityViewModel()
        {
            setLocationsLabels();
        }

        private async void setLocationsLabels()
        {
            Location = await getLocation(ConnectedActivity.JobOffer.Location);
            JobLocation = await getLocation(ConnectedActivity.JobOffer.JobLocation);
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
