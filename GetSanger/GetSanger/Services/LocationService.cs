using GetSanger.Interfaces;
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
        private IPageService m_PageService;

        public LocationService()
        {
        }

        public CancellationTokenSource Cts { get; private set; }

        public async Task<Location> GetCurrentLocation()
        {
            SetDependencies();
            Location location = null;
            bool locationGranted = await IsLocationGrantedAndAskFor() == PermissionStatus.Granted;
            if (locationGranted)
            {
                GeolocationRequest geoReq = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                Cts = new CancellationTokenSource();
                location = await Geolocation.GetLocationAsync(geoReq, Cts.Token);
            }

            return location;
        }

        public async Task<PermissionStatus> IsLocationGrantedAndAskFor()
        {
            SetDependencies();
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if(status == PermissionStatus.Granted)
            {
                return status;
            }

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await m_PageService.DisplayAlert("Error", "Please go to your settings and enable the location!", "OK");
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                await m_PageService.DisplayAlert("Note", "We need your Location to be able to give the best service with your job offer", "Thanks");
            }

            return await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }


        public async Task<bool> IsLocationGranted()
        {
            return await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted;
        } 

        public async Task<Placemark> PickedLocation(Location i_Location)
        {
            Placemark placemark = null;
            if(i_Location != null)
            {
                IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(i_Location);
                placemark = placemarks?.FirstOrDefault();
            }
           
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
            if(AppManager.Instance.ConnectedUser.UserLocation != null)
            {
                await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
            }
        }

        public override void SetDependencies()
        {
            m_PageService ??= AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
        }
    }
}
