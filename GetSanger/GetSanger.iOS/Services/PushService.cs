using System.Threading.Tasks;
using GetSanger.Interfaces;
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
    }
}