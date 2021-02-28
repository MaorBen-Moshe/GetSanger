using GetSanger.Models;
using Xamarin.Essentials;

namespace GetSanger.ViewModels
{
    public class JobOfferViewModel : BaseViewModel
    {
        private LocationService LocationServices { get; set; } = new LocationService();

        private Placemark m_MyPlacemark;
        private Placemark m_JobPlacemark;
        private string m_MyLocation;
        private string m_JobLocation;

        public Placemark MyPlaceMark
        {
            get
            {
                return m_MyPlacemark;
            }

            set
            {
                m_MyPlacemark = value;
                MyLocation = placemarkValidation(m_MyPlacemark);
            }
        }

        public Placemark JobPlaceMark
        {
            get
            {
                return m_JobPlacemark;
            }

            set
            {
                m_JobPlacemark = value;
                JobLocation = placemarkValidation(m_JobPlacemark);
            }
        }

        public string MyLocation
        {
            get { return m_MyLocation; }
            set { SetValue(ref m_MyLocation, value); }
        }

        public string JobLocation
        {
            get { return m_JobLocation; }
            set { SetValue(ref m_JobLocation, value); }
        }

        public async void GetCurrentLocation()
        {
            Location location = await LocationServices.GetCurrentLocation();
            MyPlaceMark = await LocationServices.PickedLocation(location);
        }

        private string placemarkValidation(Placemark i_Placemark)
        {
            string toRet;
            if (i_Placemark == null)
            {
                toRet = "Location could not be found, please try manually add it";
            }

            toRet = string.Format("{0}, {1} {2}", i_Placemark.Locality, i_Placemark.Thoroughfare, i_Placemark.SubThoroughfare);
            return toRet;
        }
    }
}
