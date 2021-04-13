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
        private System.Timers.Timer m_Timer;

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

        public void HandleTripThread(System.Timers.ElapsedEventHandler i_Elpased, int i_Interval)
        {
           m_Timer = new System.Timers.Timer
           {
                Interval = i_Interval //300000
           };

            m_Timer.Elapsed += i_Elpased;
            m_Timer.Start();
        }

        public void LeaveTripThread(System.Timers.ElapsedEventHandler i_Elpased)
        {
            m_Timer.Elapsed -= i_Elpased;
            m_Timer.Stop();
        }

        //// -----------------------------------------------------------------
        //// When the sanger starts his location sharing on the map,  HelperLocationUpdate method runs in a separate thread on the sanger device,
        //// and sends his location every few seconds through the server, and the client updates his map pin accordingly.


        //public void StartMapTracking()
        //{
        //    Thread t = new Thread(() => HelperLocationUpdate());
        //    t.Start();
        //}

        //public void HelperLocationUpdate()
        //{
        //    Timer timer = new Timer(new TimerCallback(SangerLocationUpdate), null, 0, 5000);
        //}


        //public async void SangerLocationUpdate(object o_State)
        //{
        //    if (!KeepThread)
        //        Cancelation();
        //    m_CurrLocation = await GetCurrentLocation();
        //    if(m_CurrLocation != m_PrevLocation && m_CurrLocation != null)
        //    {
        //        // Send m_CurrLocation to server (need to send SangerID and ClientID?)
                
        //    }
           
        //}

        //// Client side:
        //public void HelperClientActivityMapUpdate(Location o_Location)
        //{
        //    Thread t = new Thread(() => ClientActivityMapUpdate(ref o_Location));
        //    t.Start();
        //}

        //// This method requests from the server the updated location of the sanger, 
        ////    we might want to use FCM instead, so it will be handled like and event
        //public void ClientActivityMapUpdate(ref Location o_Location)
        //{
        //    // Get Sanger location from the server
        //    Location SangerLocation = null; // Swap this line with real SangerLocation from server

        //    o_Location = SangerLocation;
        //}
    }
}
