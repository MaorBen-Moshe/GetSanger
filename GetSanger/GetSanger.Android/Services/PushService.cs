﻿using Android.App;
using Android.Content;
using Android.OS;
using System.Collections.Generic;
using GetSanger.Interfaces;
using Xamarin.Forms;
using Android.Gms.Common;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Firebase.Messaging;
using GetSanger.Services;

[assembly: Dependency(typeof(GetSanger.Droid.Services.PushService))]

namespace GetSanger.Droid.Services
{
    class PushService : IPushService
    {
        internal static readonly string CHANNEL_ID = "notification_channel";

        public async Task<string> GetRegistrationToken()
        {
            Android.Gms.Tasks.Task getTokenTask = FirebaseMessaging.Instance.GetToken();
            string token = (string) await getTokenTask;

            return token;
        }

        internal async Task PushHelper(Intent intent, MainActivity invoker)
        {

            if (intent.Extras != null)
            {
                string json = null;
                string type = null;
                foreach (var key in intent.Extras.KeySet())
                {
                    var value = intent.Extras.GetString(key);
                    // We can add here more logic as needed
                    if (key == "Json")
                    {
                        json = value;
                    }
                    else if (key == "Type")
                    {
                        type = value;
                    }
                }

                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    ["Json"] = json,
                    ["Type"] = type
                };

                await PushServices.handleMessageReceived(null, null, dict);
            }

            if (!IsPlayServicesAvailable(invoker))
            {
                // Must have play services available
                int exceptionParam = 0;
                throw new GooglePlayServicesNotAvailableException(exceptionParam);
            }

            CreateNotificationChannel();
        }

        public static bool IsPlayServicesAvailable(MainActivity Invoker)
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

        public static void CreateNotificationChannel()
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
                (NotificationManager) Android.App.Application.Context.GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}