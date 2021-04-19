using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PushServices : Service
    {
        private readonly static IPushService sr_Push = DependencyService.Get<IPushService>();

        public async Task<bool> SendToDevice<T>(string i_UserId, T i_Data, string i_Message = null)
        {
            User user = await FireStoreHelper.GetUser(i_UserId);
            i_Message = user.IsGenericNotifications ? i_Message : null;
            //send push
            return true;
        }

        public bool SendTAllTopic<T>(string i_Topic, T i_Data)
        {
            return true;
        }

        public bool RegisterTopics(string i_UserId, params string[] i_Topics)
        {
            
            return true;
        }

        public bool UnsubscribeTopics(string i_UserId, params string[] i_Topics)
        {
            return true;
        }

        public override void SetDependencies()
        {
            //
        }
    }
}
