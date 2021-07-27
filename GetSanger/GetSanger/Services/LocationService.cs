using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class LocationService : Service, ILocation, ITrip
    {
        private readonly System.Timers.Timer m_Timer;
        private IPageService m_PageService;
        private static bool s_LocationSharedBySanger;

        public LocationService()
        {
            m_Timer = new System.Timers.Timer();
        }

        public CancellationTokenSource Cts { get; set; }

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

        public async Task<Placemark> GetPickedLocation(Location i_Location)
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

        public void StartTripThread(System.Timers.ElapsedEventHandler i_Elapsed = null , int i_Interval = 15000)
        {
            bool sangerShare = i_Elapsed == null;
            if (s_LocationSharedBySanger && sangerShare) 
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() => {
                m_Timer.Enabled = true;
                m_Timer.Interval = i_Interval; //15000 by default
                m_Timer.Elapsed += i_Elapsed ?? handleSangerLocation;
                m_Timer.Start();
            });

            if (sangerShare)
            {
                s_LocationSharedBySanger = true;
            }
        }

        public void LeaveTripThread(System.Timers.ElapsedEventHandler i_Elapsed = null)
        {
            bool sangerShare = i_Elapsed == null;
            if (!s_LocationSharedBySanger && sangerShare)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() => {
                m_Timer.Elapsed -= i_Elapsed ?? handleSangerLocation;
                m_Timer.Enabled = false;
                m_Timer.Stop();
            });

            if (sangerShare)
            {
                s_LocationSharedBySanger = false;
            }
        }

        public async Task<bool> TryShareSangerLoaction()
        {
            bool shared = false;
            Dictionary<string, bool> activatedMap = AppManager.Instance.ConnectedUser?.ActivatedMap;
            if(activatedMap != null)
            {
                foreach (var item in activatedMap)
                {
                    Activity activity = await FireStoreHelper.GetActivity(item.Key);
                    if (activity.SangerID.Equals(AuthHelper.GetLoggedInUserId())
                        && item.Value.Equals(true)
                        && activity.Status.Equals(eActivityStatus.Active))
                    {
                        shared = true;
                        break;
                    }
                }
            }

            return shared;
        }

        // we should start and end sanger location sharing every time he leaves the app or move to user mode
        private async void handleSangerLocation(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool sharedEnabled = await TryShareSangerLoaction();
            if (sharedEnabled == false)
            {
                LeaveTripThread();
                return;
            }

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
