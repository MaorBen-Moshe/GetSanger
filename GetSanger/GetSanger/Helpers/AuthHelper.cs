using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using GetSanger.Interfaces;
using Xamarin.Forms;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GetSanger.Helpers
{
    public static class AuthHelper
    {
        private static IAuth auth = DependencyService.Get<IAuth>();
        public static async Task RegisterViaEmail(string i_Email, string i_Password)
        {
            string idToken = await auth.GetIdToken();
            Dictionary<string, string> details = new Dictionary<string, string>()
            {
                ["email"] = i_Email,
                ["password"] = i_Password
            };

            string json = JsonSerializer.Serialize(details);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post,
                "https://europe-west3-get-sanger.cloudfunctions.net/RegisterUserWithEmailAndPassword");
            httpRequest.Content = new StringContent(json);
            httpRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(idToken);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            HttpMessageInvoker httpMessageInvoker = new HttpClient(httpClientHandler, false);

            HttpResponseMessage response = await httpMessageInvoker.SendAsync(httpRequest, new CancellationToken());
            if (response == null || !response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public static void LoginViaEmail(string i_Email, string i_Password)
        {
            throw new NotImplementedException();
        }

        public static void LoginViaGoogle()
        {
            throw new NotImplementedException();
        }

        public static void LoginViaFacebook()
        {
            throw new NotImplementedException();
        }

        public static void AddUser(User i_User)
        {
            throw new NotImplementedException();
        }

        public static bool IsLoggedIn()
        {
            throw new NotImplementedException();
        }

        public static int GetLoggedInUserID()
        {
            throw new NotImplementedException();
        }
    }
}
