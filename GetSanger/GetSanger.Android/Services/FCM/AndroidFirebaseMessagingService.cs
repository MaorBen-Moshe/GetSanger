using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;
using System.Collections.Generic;
using Android.Support.V4.App;
using System.Threading.Tasks;
using Android.Widget;
using GetSanger.Services;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace GetSanger.Droid.Services.FCM
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class AndroidFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message)
        {//Sets how the app handles messages and notifications in foreground (background messages are not passed here)
            base.OnMessageReceived(message);
            //SendNotification(message.GetNotification().Body, message.Data);
            DataManipulationExample(message);
        }

        public override void OnNewToken(string newToken)
        {
            base.OnNewToken(newToken);
            SendRegistrationToServer(newToken);
        }
        
        void SendRegistrationToServer(string newToken)
        {
            // Add custom implementation, as needed.
            // Server should resubscribe the user to the previous topics he was subscribed to
        }

        void SendNotification(string messageBody, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(this,
                                                          MainActivity.NOTIFICATION_ID,
                                                          intent,
                                                          PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.notificationPic)
                                      .SetContentTitle("FCM Message")
                                      .SetContentText(messageBody)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }

        internal async void DataManipulationExample(RemoteMessage remoteMessage)
        {
            string pageToNavigate = "";
            foreach (var key in remoteMessage.Data.Keys)
            {
                if (key == "type")
                {
                    pageToNavigate = remoteMessage.Data[key];
                    break;
                }
            }
            NavigationService nservice = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            bool choice = false;
            if (AppManager.Instance.CurrentMode == AppMode.Client)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    choice = await App.Current.MainPage.DisplayAlert
                     (string.Format("Move to ", pageToNavigate, "?"), "Do you wish to navigate to the page?", "Yes", "No");
                    if (choice)
                    {
                        if (pageToNavigate == ShellRoutes.JobOffer)
                        {
                            // navigate to Mode page, if the user swaps to Sanger mode, then navigate to the JobOffer page
                            await nservice.NavigateTo("mode");
                            // Need more logic

                        }
                        else if (pageToNavigate == "signupEmail")
                        {
                            await nservice.NavigateTo(ShellRoutes.SignupEmail);
                        }

                    }
                });
            }
            else if (AppManager.Instance.CurrentMode == AppMode.Sanger)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    choice = await App.Current.MainPage.DisplayAlert
                     (string.Format("Move to ", pageToNavigate, "?"), "Do you wish to navigate to the page?", "Yes", "No");
                    if (choice)
                    {

                    }
                });
            }
        }
    }
}