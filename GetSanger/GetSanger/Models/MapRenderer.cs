using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace GetSanger.Models
{
    class MapRenderer
    {
        private Geocoder Geo { get; set; }

        private CancellationTokenSource Cts { get; set; }

        public MapRenderer()
        {
            Geo = new Geocoder();
        }

        public async Task<Placemark> PickedLocation(Location i_Location)
        {
            IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(i_Location);
            Placemark placemark = placemarks.FirstOrDefault();
            return placemark;
        }

        public async Task<Location> GetCurrentLocation()
        {
            try
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
            catch (FeatureNotSupportedException ex)
            {
                throw new NotImplementedException();
            }
            catch (FeatureNotEnabledException ex)
            {
                throw new NotImplementedException();
            }
            catch (PermissionException ex)
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
