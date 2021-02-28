using System.Collections.Generic;
using System.Linq;
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

        public async Task<Xamarin.Forms.Maps.Map> CreateMapAsync(Position? i_Position = null)
        {
            Position position;
            if (i_Position == null)
            {
                Location location = await MapRend.GetCurrentLocation();
                position = new Position(location.Latitude, location.Longitude);
            }
            else
            {
                position = (Position)i_Position;
            }
          
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
            string location = $"Did you choose the right place?\n {string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare)}";
            bool answer = await m_PageService.DisplayAlert("Location Chosen", location, "Yes", "No");
            if (answer)
            {
                (ConnecetedPage as JobOfferViewModel).JobPlaceMark = placemark;
            }

            await m_PageService.PopAsync();
        }

        public async Task<Xamarin.Forms.Maps.Map> SetSearch(string i_Search)
        {
            Position position;
            Xamarin.Forms.Maps.Map map = null;
            List<Position> positionList = new List<Position>(await (new Geocoder()).GetPositionsForAddressAsync(i_Search));
            if(positionList.Count != 0)
            {
                position = positionList.FirstOrDefault<Position>();
                await CreateMapAsync(position);
                map = await CreateMapAsync(position);
                map.Pins.Add(new Pin
                {
                    Type = PinType.Place,
                    Position = position
                });
                map.MoveToRegion(new MapSpan(position, 0.1, 0.1));
            }

            return map;
        }

        public void Cancelation()
        {
            MapRend.Cancelation();
        }
    }
}
