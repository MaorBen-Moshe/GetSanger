using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Models.chat;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GetSanger.Exceptions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PushServices : Service
    {
        private static readonly IPushService sr_Push = DependencyService.Get<IPushService>();

        public static Dictionary<string, string> BackgroundPushData { get; } = new Dictionary<string, string>();

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

        public static async Task handleMessageReceived(string i_Title, string i_Body, IDictionary<string, string> i_Message)
        {
            if (i_Message != null && i_Message.ContainsKey("Type"))
            {
                string type = i_Message["Type"];
                string json = i_Message["Json"];
                string mode = i_Message.ContainsKey("Mode") ? i_Message["Mode"] : null;
                i_Message.Clear();

                switch (type)
                {
                    case nameof(JobOffer):
                        await handleJobOffer(i_Title, i_Body, json);
                        break;
                    case nameof(Activity):
                        await handleActivity(i_Title, i_Body, json, mode);
                        break;
                    case nameof(Message):
                        await handleMessage(i_Title, i_Body, json);
                        break;
                    case nameof(Rating):
                        await handleRating(i_Title, i_Body, json);
                        break;
                    default:
                        throw new ArgumentException("Type of object received is not allowed");
                }
            }
        }

        private static async Task handleRating(string i_Title, string i_Body, string i_Json)
        {
            bool choice = true;
            string message = i_Body + "\nGo to profile page to view your ratings?";
            //Rating rating = ObjectJsonSerializer.DeserializeForServer<Rating>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            // string ratingJson = ObjectJsonSerializer.SerializeForPage(rating);
            if (i_Title != null)
            {
                choice = await Application.Current.MainPage.DisplayAlert(i_Title, message, "Yes", "No");
            }

            if (choice)
            {
                AppManager.Instance.CurrentMode = AppManager.Instance.ConnectedUser.LastUserMode.GetValueOrDefault(AppMode.Client);
                Application.Current.MainPage = AppManager.Instance.GetCurrentShell();
                await navigation.NavigateTo($"{ShellRoutes.Ratings}?isMyRatings={true}&id={AppManager.Instance.ConnectedUser.UserId}");
            }
        }

        private static async Task handleMessage(string i_Title, string i_Body, string i_Json)
        {
            bool choice = true;
            string txt = i_Body + "\nMove to chat page?";
            Message message = ObjectJsonSerializer.DeserializeForServer<Message>(i_Json);
            ChatDatabase.ChatDatabase db = await ChatDatabase.ChatDatabase.Instance;
            await db.AddMessageAsync(message, message.FromId);

            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            if (i_Title != null)
            {
                choice = await Application.Current.MainPage.DisplayAlert(i_Title, txt, "Yes", "No");
            }

            if (choice == true)
            {
                await navigation.NavigateTo(ShellRoutes.ChatView + $"?user={message.FromId}");
            }
        }

        private static async Task handleActivity(string i_Title, string i_Body, string i_Json, string i_Mode)
        {
            bool choice = true;
            AppMode mode = i_Mode != null ? Enum.Parse<AppMode>(i_Mode) : AppManager.Instance.CurrentMode;
            string message = i_Body + "\nDo you want to navigate to view the activity?";
            Activity activity = ObjectJsonSerializer.DeserializeForServer<Activity>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            if (i_Title != null)
            {
                choice = await Application.Current.MainPage.DisplayAlert(i_Title, message, "Yes", "No");
            }

            if (choice)
            {
                AppManager.Instance.CurrentMode = mode;
                AppManager.Instance.ConnectedUser.LastUserMode = mode;
                await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                Application.Current.MainPage = AppManager.Instance.GetCurrentShell(mode);

                await navigation.NavigateTo(ShellRoutes.Activity + $"?activity={ObjectJsonSerializer.SerializeForPage(activity)}");
            }
        }

        private static async Task handleJobOffer(string i_Title, string i_Body, string i_Json)
        {
            bool choice = true;
            string message = i_Body + "\nDo you want to navigate the the job offer page?";
            JobOffer job = ObjectJsonSerializer.DeserializeForServer<JobOffer>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            string jobJson = ObjectJsonSerializer.SerializeForPage(job);
            if (i_Title != null)
            {
                choice = await Application.Current.MainPage.DisplayAlert(i_Title, message, "Yes", "No");
            }

            if (choice)
            {
                AppManager.Instance.CurrentMode = AppMode.Sanger;
                AppManager.Instance.ConnectedUser.LastUserMode = AppMode.Sanger;
                await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                Application.Current.MainPage = AppManager.Instance.GetCurrentShell(AppMode.Sanger);

                await navigation.NavigateTo(ShellRoutes.ViewJobOffer + $"?jobOffer={jobJson}");
            }
        }

        public async Task<bool> IsRegistrationTokenChanged()
        {
            User connectedUser = AppManager.Instance.ConnectedUser;

            return await GetRegistrationToken() != connectedUser.RegistrationToken;
        }

        public async void UnsubscribeUser(string i_UserId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/UnsubscribeUser";

                Dictionary<string, string> requestDictionary = new Dictionary<string, string>()
                {
                    ["UserId"] = i_UserId
                };

                string json = ObjectJsonSerializer.SerializeForServer(requestDictionary);
                string idToken = await AuthHelper.GetIdTokenAsync();

                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public async void SubscribeUser(string i_UserId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SubscribeUser";

                Dictionary<string, string> requestDictionary = new Dictionary<string, string>()
                {
                    ["UserId"] = i_UserId
                };

                string json = ObjectJsonSerializer.SerializeForServer(requestDictionary);
                string idToken = await AuthHelper.GetIdTokenAsync();

                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }
    }
}