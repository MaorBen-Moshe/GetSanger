using System.Threading.Tasks;
using System.Windows.Input;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GetSanger.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private IPageService m_PageService;

        private BaseViewModel ConnecetedPage { get; set; }

        public LocationService MapRend { get; private set; }

        public MapViewModel(BaseViewModel i_RefPage)
        {
            MapRend = new LocationService();
            m_PageService = new PageServices();
            ConnecetedPage = i_RefPage;
        }

        public async Task<Xamarin.Forms.Maps.Map> CreateMapAsync()
        {
            Location location = await MapRend.GetCurrentLocation();
            Position position = new Position(location.Latitude, location.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);

            Xamarin.Forms.Maps.Map map = new Xamarin.Forms.Maps.Map(mapSpan)
            {
                MapType = MapType.Street,
                IsShowingUser = true
            };

            return map;
        }

        public async void MapClicked(Position i_Position)
        {
            Placemark placemark = await MapRend.PickedLocation(new Location(i_Position.Latitude, i_Position.Longitude));
            string location = $"Did you choose the right place?\n {placemark}";
            bool answer = await m_PageService.DisplayAlert("Location Chosen", location, "Yes", "No");
            if (answer)
            {
                (ConnecetedPage as JobOfferViewModel).JobPlaceMark = placemark;
            }
        }

        public void Cancelation()
        {
            MapRend.Cancelation();
        }
    }
}
