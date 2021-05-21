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
        private static readonly IPushService sr_Push = DependencyService.Get<IPushService>();

        public async void SendToDevice<T>(string i_UserId, T i_Data, string i_Message = null)
        {
            User user = await FireStoreHelper.GetUser(i_UserId);
            i_Message = user.IsGenericNotifications ? i_Message : null;
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SendPushToToken";

            Dictionary<string, object> pushData = new Dictionary<string, object>
            {
                ["UserId"] = i_UserId,
                ["Data"] = i_Data,
                ["Body"] = i_Message,
                ["Title"] = "New notification"
            };

            string json = JsonSerializer.Serialize(pushData);
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

            string json = JsonSerializer.Serialize(pushData);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        // i_Topics = a representation of the int value of the enum or "Generic" string that represent generic notifications
        public async void UnsubscribeTopics(string i_UserId, params string[] i_Topics)
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/UnsubscribeFromTopics";
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
                throw new NotImplementedException("In HandeDataReceived");
            }
            else
            {
                throw new ArgumentException("Type of object received is not allowed");
            }
        }

        private async static void handleMessage(string i_Json)
        {
            Message message = JsonSerializer.Deserialize<Message>(i_Json);
            ChatDatabase.ChatDatabase db = AppManager.Instance.Services.GetService(typeof(ChatDatabase.ChatDatabase)) as ChatDatabase.ChatDatabase;
            checkIfFirstMessageReceived(message, db);
            await db.SaveItemAsync(message, message.FromId);
        }

        private async static void checkIfFirstMessageReceived(Message i_Message, ChatDatabase.ChatDatabase i_DB)
        {
            bool found = false;
            List<string> usersInDB = await i_DB.GetUsersIDsInDB();
            foreach(var chatUserID in usersInDB)
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
            Activity activity = JsonSerializer.Deserialize<Activity>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            bool choice = await App.Current.MainPage.DisplayAlert
                     ("Move to activity?", "Do you wish to navigate to the page?", "Yes", "No");
            if (choice)
            {
                await navigation.NavigateTo(ShellRoutes.Activity + $"?activity={activity}");
            }
        }

        private async static void handleJobOffer(string i_Json)
        {
            JobOffer job = JsonSerializer.Deserialize<JobOffer>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            bool choice = await App.Current.MainPage.DisplayAlert
                     ("Move to job offer?", "Do you wish to navigate to the page?", "Yes", "No");
            if (choice)
            {
                await navigation.NavigateTo(ShellRoutes.JobOffer + $"?jobOffer={job}&isCreate={false}&category={job.Category}");
            }
        }

        private static Type getTypeOfData(string i_Type)
        {
            Type type = null;
            if (i_Type.Equals(typeof(JobOffer).Name.ToString()))
            {
                type = typeof(JobOffer);
            }
            else if(i_Type.Equals(typeof(Activity).Name.ToString()))
            {
                type = typeof(Activity);
            }
            else if(i_Type.Equals(typeof(Rating).Name.ToString()))
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