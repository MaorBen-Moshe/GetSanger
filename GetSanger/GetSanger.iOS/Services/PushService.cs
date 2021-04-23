using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using GetSanger.Interfaces;
using Xamarin.Forms;
using GetSanger.iOS.Services;

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
            throw new NotImplementedException();
            //return FCMToken;
        }
    }
}