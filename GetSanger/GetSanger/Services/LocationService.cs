using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GetSanger.Services
{
    public class LocationService
    {
        public CancellationTokenSource Cts { get; private set; }

        public async Task<Location> GetCurrentLocation()
        {
            Location location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                GeolocationRequest geoReq = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30));
                Cts = new CancellationTokenSource();
                location = await Geolocation.GetLocationAsync(geoReq, Cts.Token);
            }

            return location;
        }

        public async Task<Placemark> PickedLocation(Location i_Location)
        {
            IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(i_Location);
            Placemark placemark = placemarks.FirstOrDefault();
            return placemark;
        }

        public void Cancelation()
        {
            if (Cts != null && !Cts.IsCancellationRequested)
            {
                Cts.Cancel();
            }
        }
    }
}
