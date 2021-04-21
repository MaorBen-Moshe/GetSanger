using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GetSanger.Services
{
    public class LocationService : Service
    {
        private System.Timers.Timer m_Timer;

        public LocationService()
        {
        }

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

        public void StartTripThread(System.Timers.ElapsedEventHandler i_Elpased = null , int i_Interval = 300000)
        {
           m_Timer = new System.Timers.Timer
           {
                Interval = i_Interval //300000
           };

            m_Timer.Elapsed += i_Elpased ?? handleSangerLocation;
            m_Timer.Start();
        }

        public void LeaveTripThread(System.Timers.ElapsedEventHandler i_Elpased = null)
        {
            m_Timer.Elapsed -= i_Elpased ?? handleSangerLocation;
            m_Timer.Stop();
        }

        // we should start and end sanger location sharing every time he leaves the app or move to user mode
        private async void handleSangerLocation(object sender, System.Timers.ElapsedEventArgs e)
        {
            AppManager.Instance.ConnectedUser.UserLocation = await GetCurrentLocation();
            await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
        }

        public override void SetDependencies()
        {
            //
        }
    }
}
