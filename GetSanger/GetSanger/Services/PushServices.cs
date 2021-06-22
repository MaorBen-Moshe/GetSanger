using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Models.chat;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PushServices : Service
    {
        private static readonly IPushService sr_Push = DependencyService.Get<IPushService>();

        public async Task SendToDevice<T>(string i_UserId, T i_Data, Type i_DataType, string i_Title = "", string i_Message = null) where T : class
        {
            User user = await FireStoreHelper.GetUser(i_UserId);
            i_Message = user.IsGenericNotifications ? i_Message : null;
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SendPushToToken";

            string dataJson = ObjectJsonSerializer.SerializeForServer(i_Data);
            Dictionary<string, string> data = null;
            if (i_Data != null)
            {
                data = new Dictionary<string, string>
                {
                    ["Type"] = i_DataType?.ToString(),
                    ["Json"] = dataJson
                };
            }


            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["UserId"] = i_UserId,
                ["Data"] = data,
                ["Body"] = i_Message,
                ["Title"] = i_Title
            };

            string json = ObjectJsonSerializer.SerializeForServer(pushData);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        // i_Topics = a representation of the int value of the enum or "Generic" string that represent generic notifications
        public async Task RegisterTopics(string i_UserId, params string[] i_Topics)
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SubscribeToTopics";
            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["UserId"] = i_UserId,
                ["Topics"] = i_Topics
            };

            string json = ObjectJsonSerializer.SerializeForServer(pushData);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        // i_Topics = a representation of the int value of the enum or "Generic" string that represent generic notifications
        public async Task UnsubscribeTopics(string i_UserId, params string[] i_Topics)
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/UnsubscribeFromTopics";
            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["UserId"] = i_UserId,
                ["Topics"] = i_Topics
            };

            string json = ObjectJsonSerializer.SerializeForServer(pushData);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public override void SetDependencies()
        {
            // do nothing
        }

        public async Task<string> GetRegistrationToken()
        {
            return await sr_Push.GetRegistrationToken();
        }

        public static void HandleDataReceived(IDictionary<string, string> i_Message)
        {
            Type type = getTypeOfData(i_Message["Type"]);
            if (type.Equals(typeof(JobOffer)))
            {
                handleJobOffer(i_Message["Json"]);
            }
            else if (type.Equals(typeof(Models.Activity)))
            {
                handleActivity(i_Message["Json"]);
            }
            else if (type.Equals(typeof(Models.chat.Message)))
            {
                handleMessage(i_Message["Json"]);
            }
            else
            {
                throw new ArgumentException("Type of object received is not allowed");
            }
        }

        private async static void handleMessage(string i_Json)
        {
            Message message = ObjectJsonSerializer.DeserializeForServer<Message>(i_Json);
            ChatDatabase.ChatDatabase db = AppManager.Instance.Services.GetService(typeof(ChatDatabase.ChatDatabase)) as ChatDatabase.ChatDatabase;
            checkIfFirstMessageReceived(message, db);
            await db.SaveItemAsync(message, message.FromId);
        }

        private async static void checkIfFirstMessageReceived(Message i_Message, ChatDatabase.ChatDatabase i_DB)
        {
            bool found = false;
            List<string> usersInDB = await i_DB.GetUsersIDsInDB();
            foreach (var chatUserID in usersInDB)
            {
                if (chatUserID.Equals(i_Message.FromId))
                {
                    found = true;
                    break;
                }
            }

            if (!found) // first time
            {
                await i_DB.SaveUserAsync(i_Message.FromId);
            }
        }

        private async static void handleActivity(string i_Json)
        {
            Activity activity = ObjectJsonSerializer.DeserializeForServer<Activity>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            await navigation.NavigateTo(ShellRoutes.Activity + $"?activity={ObjectJsonSerializer.SerializeForPage(activity)}");
        }

        private async static void handleJobOffer(string i_Json)
        {
            JobOffer job = ObjectJsonSerializer.DeserializeForServer<JobOffer>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            string jobJson = ObjectJsonSerializer.SerializeForPage(job);
            await navigation.NavigateTo(ShellRoutes.ViewJobOffer +
                                        $"?jobOffer={jobJson}");
        }

        private static Type getTypeOfData(string i_Type)
        {
            Type type = null;
            if (i_Type.Equals(typeof(JobOffer).Name.ToString()))
            {
                type = typeof(JobOffer);
            }
            else if (i_Type.Equals(typeof(Activity).Name.ToString()))
            {
                type = typeof(Activity);
            }
            else if (i_Type.Equals(typeof(Rating).Name.ToString()))
            {
                type = typeof(Rating);
            }
            else if (i_Type.Equals(typeof(Message).Name.ToString()))
            {
                type = typeof(Message);
            }
            else
            {
                throw new ArgumentException("Type of object received is not allowed");
            }

            return type;
        }
    }
}