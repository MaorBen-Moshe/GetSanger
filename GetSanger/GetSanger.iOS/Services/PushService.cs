using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using GetSanger.Interfaces;
using GetSanger.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Firebase.CloudMessaging.Messaging;

[assembly: Dependency(typeof(GetSanger.iOS.Services.PushService))]

namespace GetSanger.iOS.Services
{
    class PushService : IPushService
    {
        private static string m_FCMToken = null;
        internal static readonly string CHANNEL_ID = "notification_channel";

        public static string FCMToken
        {
            get { return m_FCMToken; }
            set { m_FCMToken = value; }
        }

        public Task<string> GetRegistrationToken()
        {
            string fcmToken = SharedInstance.FcmToken;
            return Task.FromResult(fcmToken);
        }

        internal static void PushHelper(NSDictionary message)
        {
            if (!Forms.IsInitialized)
            {
                Forms.Init();
            }

            Dictionary<string, string> backgroundPushData = PushServices.BackgroundPushData;
            backgroundPushData.Clear();

            foreach (var key in message.Keys)
            {
                string keyStr = key.ToString();
                var value = message[key].ToString();

                if (keyStr == "Json")
                {
                    backgroundPushData["Json"] = value;
                }
                else if (keyStr == "Type")
                {
                    backgroundPushData["Type"] = value;
                }
            }

            if (message.ContainsKey(new NSString("aps")))
            {
                var apsDictionary = message["aps"] as NSDictionary;
                string title = null, body = null;
                if (apsDictionary.ContainsKey(new NSString("alert")) && apsDictionary["alert"] is NSDictionary alertDictionary)
                {
                    title = alertDictionary["title"].ToString();
                    body = alertDictionary["body"].ToString();
                }

                MainThread.BeginInvokeOnMainThread(async () => await PushServices.HandleMessageReceived(title, body, backgroundPushData));
            }
        }
    }
}