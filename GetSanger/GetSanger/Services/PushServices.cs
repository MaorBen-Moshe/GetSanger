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
using GetSanger.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using GetSanger.Views.chat;
using GetSanger.ViewModels.chat;
using GetSanger.ViewModels;
using GetSanger.Views;

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
            string token = await sr_Push.GetRegistrationToken();
            return token;
        }

        public static async Task handleMessageReceived(string i_Title, string i_Body, IDictionary<string, string> i_Message)
        {
            if (i_Message != null && i_Message.ContainsKey("Type"))
            {
                string type = i_Message["Type"];
                string json = i_Message.ContainsKey("Json") ? i_Message["Json"] : null;
                i_Message.Clear();

                switch (type)
                {
                    case nameof(JobOffer):
                        await handleJobOffer(i_Title, i_Body, json);
                        break;
                    case nameof(Activity):
                        await handleActivity(i_Title, i_Body, json);
                        break;
                    case nameof(Message):
                        await handleMessage(json);
                        break;
                    case nameof(Rating):
                        await handleRating(i_Title, i_Body, json);
                        break;
                    case "MessageInfo":
                        await handleMessageInfo(i_Title, i_Body, json);
                        break;
                    default:
                        throw new ArgumentException("Type of object received is not allowed");
                }
            }
        }

        private static async Task handleRating(string i_Title, string i_Body, string i_Json)
        {
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            bool choice = true;
            string message = i_Body + "\nGo to profile page to view your ratings?";

            Page currentPage = Shell.Current.CurrentPage;

            if (currentPage is {BindingContext: RatingsViewModel {IsMyRatings: true} vm})
            {
                vm.RefreshingCommand.Execute(null);
            }
            else
            {
                if (i_Title != null)
                {
                    choice = await Application.Current.MainPage.DisplayAlert(i_Title, message, "Yes", "No");
                }

                if (choice)
                {
                    await navigation.NavigateTo(
                        $"//account/{ShellRoutes.Ratings}?isMyRatings={true}&id={AppManager.Instance.ConnectedUser.UserId}");
                }
            }
        }

        private static async Task handleMessageInfo(string i_Title, string i_Body, string i_Json)
        {
            bool choice = true;
            string txt = i_Body + "\nMove to chat page?";

            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            Message message = ObjectJsonSerializer.DeserializeForServer<Message>(i_Json);
            string senderId = message.FromId;
            User sender = await FireStoreHelper.GetUser(senderId);

            Page currentPage = Shell.Current.CurrentPage;
            if (currentPage is {BindingContext: ChatPageViewModel vm})
            {
                if (vm.UserToChat.UserId == senderId)
                {
                    vm.HandleMessageReceivedCommand.Execute(message);
                }
                else
                {
                    if (i_Title != null)
                    {
                        choice = await Application.Current.MainPage.DisplayAlert(i_Title, txt, "Yes", "No");
                    }

                    if (choice)
                    {
                        await navigation.NavigateTo($"../{ShellRoutes.ChatView}?user={ObjectJsonSerializer.SerializeForPage(sender)}");
                    }
                }
            }
            else
            {
                if (i_Title != null)
                {
                    choice = await Application.Current.MainPage.DisplayAlert(i_Title, txt, "Yes", "No");
                }

                if (choice)
                {
                    await navigation.NavigateTo($"//chatList/{ShellRoutes.ChatView}?user={ObjectJsonSerializer.SerializeForPage(sender)}");
                }
            }
        }

        private static async Task handleMessage(string i_Json)
        {
            Message message = ObjectJsonSerializer.DeserializeForServer<Message>(i_Json);
            message.MessageSent = true;
            ChatDatabase.ChatDatabase db = await ChatDatabase.ChatDatabase.Instance;
            await db.AddMessageAsync(message, message.FromId, message.ToId);
        }

        private static async Task handleActivity(string i_Title, string i_Body, string i_Json)
        {
            bool choice = true;
            string message = i_Body + "\nDo you want to navigate to view the activity?";
            Activity activity = ObjectJsonSerializer.DeserializeForServer<Activity>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;

            Page currentPage = Shell.Current.CurrentPage;
            if (currentPage is {BindingContext: ActivityViewModel vm} && vm.ConnectedActivity.ActivityId == activity.ActivityId)
            {
                // execute vm refresh command
            }
            else
            {
                if (i_Title != null)
                {
                    choice = await Application.Current.MainPage.DisplayAlert(i_Title, message, "Yes", "No");
                }

                if (choice)
                {
                    eAppMode modeToSet = activity.ClientID == AppManager.Instance.ConnectedUser.UserId ? eAppMode.Client : eAppMode.Sanger;
                    Application.Current.MainPage = AppManager.Instance.GetCurrentShell(modeToSet);

                    string activityJson = ObjectJsonSerializer.SerializeForPage(activity);
                    await navigation.NavigateTo($"//activities/{ShellRoutes.Activity}?activity={activityJson}");
                }
            }
        }

        private static async Task handleJobOffer(string i_Title, string i_Body, string i_Json)
        {
            bool choice = true;
            string message = i_Body + "\nDo you want to navigate the the job offer page?";
            JobOffer job = ObjectJsonSerializer.DeserializeForServer<JobOffer>(i_Json);
            NavigationService navigation = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;

            if (i_Title != null)
            {
                choice = await Application.Current.MainPage.DisplayAlert(i_Title, message, "Yes", "No");
            }

            if (choice)
            {
                Application.Current.MainPage = AppManager.Instance.GetCurrentShell(eAppMode.Sanger);
                string jobJson = ObjectJsonSerializer.SerializeForPage(job);
                await navigation.NavigateTo($"//jobOffers/{ShellRoutes.ViewJobOffer}?jobOffer={jobJson}");
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

                await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
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

        public async void SendChatMessage(Message i_Message) // no need for await because it's not the user's fault if there is an error
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SendChatMessage";

                string json = ObjectJsonSerializer.SerializeForServer(i_Message);
                string idToken = await AuthHelper.GetIdTokenAsync();

                await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }
    }
}