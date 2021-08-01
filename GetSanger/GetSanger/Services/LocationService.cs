﻿using GetSanger.Interfaces;
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

        public LocationService()
        {
            m_Timer = new System.Timers.Timer();
        }

        public CancellationTokenSource Cts { get; set; }

        public async Task<Location> GetCurrentLocation(bool askFor = true)
        {
            SetDependencies();
            Location location = null;
            bool locationGranted = await IsLocationGrantedAndAskFor(askFor) == PermissionStatus.Granted;
            if (locationGranted)
            {
                GeolocationRequest geoReq = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                Cts = new CancellationTokenSource();
                location = await Geolocation.GetLocationAsync(geoReq, Cts.Token);
            }

            return location;
        }

        public async Task<PermissionStatus> IsLocationGrantedAndAskFor(bool askFor = true)
        {
            SetDependencies();
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if(status == PermissionStatus.Granted)
            {
                return status;
            }

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (askFor)
                {
                    await m_PageService.DisplayAlert("Error", "Please go to your settings and enable the location!", "OK");
                }

                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                if (askFor)
                {
                    await m_PageService.DisplayAlert("Note", "We need your Location to be able to give the best service with your job offer", "Thanks");
                }
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

        public void StartTripThread(System.Timers.ElapsedEventHandler i_Elapsed, int i_Interval = 15000)
        {
            if (i_Elapsed == null)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                m_Timer.Enabled = true;
                m_Timer.Interval = i_Interval; //15000 by default
                m_Timer.Elapsed += i_Elapsed;
                m_Timer.Start();
            });
        }

        public void LeaveTripThread(System.Timers.ElapsedEventHandler i_Elapsed)
        {
            if(i_Elapsed == null)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() => {
                m_Timer.Elapsed -= i_Elapsed;
                m_Timer.Enabled = false;
                m_Timer.Stop();
            });
        }

        public override void SetDependencies()
        {
            m_PageService ??= AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
        }
    }
}