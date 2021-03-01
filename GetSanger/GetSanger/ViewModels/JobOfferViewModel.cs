using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.UI_pages.common;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class JobOfferViewModel : BaseViewModel
    {
        private LocationService LocationServices { get; set; } = new LocationService();
        private Placemark m_MyPlacemark;
        private Placemark m_JobPlacemark;
        private string m_MyLocation;
        private string m_JobLocation;
        private IPageService m_PageService;

        public ICommand CurrentLocation { get; private set; }
        public ICommand JobLocation { get; private set; }

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
                WorkLocation = placemarkValidation(m_JobPlacemark);
            }
        }

        public string MyLocation
        {
            get { return m_MyLocation; }
            set { SetClassProperty(ref m_MyLocation, value); }
        }

        public string WorkLocation
        {
            get { return m_JobLocation; }
            set { SetClassProperty(ref m_JobLocation, value); }
        }

        public JobOfferViewModel()
        {
            CurrentLocation = new Command(GetCurrentLocation);
            JobLocation = new Command(GetJobLocation);
            m_PageService = new PageServices();
        }

        public async void GetCurrentLocation()
        {
            Location location = await LocationServices.GetCurrentLocation();
            MyPlaceMark = await LocationServices.PickedLocation(location);
        }

        public async void GetJobLocation()
        {
            await m_PageService.PushAsync(new MapPage(this));
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
