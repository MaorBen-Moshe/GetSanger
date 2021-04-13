using GetSanger.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PushServices
    {
        private readonly static IPushService sr_Push = DependencyService.Get<IPushService>();

        public bool SendToDevice<T>(string i_UserId, T i_Data, string i_Message = null)
        {

            return true;
        }

        public bool SendTAllTopic<T>(string i_Topic, T i_Data)
        {
            return true;
        }

        public bool RegisterTopic(string i_UserId, IEnumerable<string> i_Topics)
        {
            
            return true;
        }

        public bool UnsubscribeTopic(string i_UserId, string i_Topic)
        {
            return true;
        }

    }
}
