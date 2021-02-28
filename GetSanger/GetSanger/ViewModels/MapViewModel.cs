using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GetSanger.Models;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace GetSanger.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        public LocationService MapRend { get; private set; }

        public MapViewModel()
        {
            MapRend = new LocationService();
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

        public async Task<Placemark> GetLocation(double i_Latitude, double i_Longitude)
        {
            return await MapRend.PickedLocation(new Location(i_Latitude, i_Longitude));
        } 

        public void Cancelation()
        {
            MapRend.Cancelation();
        }
    }
}
