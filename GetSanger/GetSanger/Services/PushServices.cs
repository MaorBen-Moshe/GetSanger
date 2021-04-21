using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PushServices : Service
    {
        private readonly static IPushService sr_Push = DependencyService.Get<IPushService>();

        public async void SendToDevice<T>(string i_UserId, T i_Data, string i_Message = null)
        {
            User user = await FireStoreHelper.GetUser(i_UserId);
            i_Message = user.IsGenericNotifications ? i_Message : null;
            string uri = "Cloud uri here!";
            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["Data"] = i_Data,
                ["message"] = i_Message
            };

            string json = JsonSerializer.Serialize(pushData);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async void SendTAllTopic<T>(string i_Topic, T i_Data, string i_Message = null)
        {
            string uri = "Cloud uri here!";
            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["Topic"] = i_Topic,
                ["Data"] = i_Data,
                ["Message"] = i_Message
            };

            string json = JsonSerializer.Serialize(pushData);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task RegisterTopics(string i_UserId, params string[] i_Topics)
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SubscribeToTopics";
            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["UserId"] = i_UserId,
                ["Topics"] = i_Topics
            };

            string json = JsonSerializer.Serialize(pushData);
            string idToken = await AuthHelper.GetIdTokenAsync();
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async void UnsubscribeTopics(string i_UserId, params string[] i_Topics)
        {
            string uri = "Cloud uri here!";
            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["UserId"] = i_UserId,
                ["Topics"] = i_Topics
            };

            string json = JsonSerializer.Serialize(pushData);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public override void SetDependencies()
        {
            // do nothing
        }

        public string GetRegistrationToken()
        {
            return sr_Push.GetRegistrationToken();
        }
    }
}