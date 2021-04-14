﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetSanger.Interfaces;
using Xamarin.Forms;
using GetSanger.Droid.Services;
using Android.Gms.Common;
using Android.Content.PM;
using ImageCircle.Forms.Plugin.Droid;
using System.Threading.Tasks;
using System.IO;

[assembly: Dependency(typeof(GetSanger.Droid.Services.PushService))]
namespace GetSanger.Droid.Services
{
    class PushService : IPushService
    {
        internal static readonly string CHANNEL_ID = "notification_channel";

        public void TempMethod(string token)
        {
            throw new NotImplementedException();
        }

        internal void PushHelper(Intent intent, MainActivity invoker)
        {
            if (intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                {
                    var value = intent.Extras.GetString(key);
                    // We can add here more logic as needed
                }
            }

            if (!IsPlayServicesAvailable(invoker))
            {
                // Must have play services available
                int exceptionParam = 0;
                throw new GooglePlayServicesNotAvailableException(exceptionParam);
            }
            CreateNotificationChannel();
        }

        public bool IsPlayServicesAvailable(MainActivity Invoker)
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Invoker);
            if (resultCode != ConnectionResult.Success)
            {

                //"This device is not supported"; 
                //Finish();

                return false;
            }
            else
            {
                //"Google Play Services is available.";
                return true;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                "FCM Notifications",
                NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager =
                (NotificationManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}