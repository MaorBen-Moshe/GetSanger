using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GetSanger.Interfaces;
using GetSanger.Models;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public static class AuthHelper
    {
        private static readonly IAuth s_Auth = DependencyService.Get<IAuth>();
        public static async Task RegisterViaEmail(string i_Email, string i_Password)
        {
            if (!s_Auth.IsAnonymousUser())
            {
                s_Auth.SignOut();
            }

            Dictionary<string, string> details = new Dictionary<string, string>()
            {
                ["email"] = i_Email,
                ["password"] = i_Password
            };

            string idToken = await s_Auth.GetIdToken();
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/RegisterUserWithEmailAndPassword";

            string json = JsonSerializer.Serialize(details);

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(responseMessage);
            }
            await LoginViaEmail(i_Email, i_Password);
        }

        public static async Task LoginViaEmail(string i_Email, string i_Password)
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SignInWithPassword";
            Dictionary<string, string> details = new Dictionary<string, string>()
            {
                ["email"] = i_Email,
                ["password"] = i_Password
            };

            string json = JsonSerializer.Serialize(details);

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);

            if (!response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(responseMessage);
            }

            string customToken = await response.Content.ReadAsStringAsync();
            s_Auth.SignInWithCustomToken(customToken);
        }

        public static void LoginViaGoogle()
        {
            throw new NotImplementedException();
        }

        public static void LoginViaFacebook()
        {
            throw new NotImplementedException();
        }

        public static bool IsLoggedIn()
        {
            return s_Auth.IsLoggedIn();
        }

        public static string GetLoggedInUserId()
        {
            return s_Auth.GetUserId();
        }

        public static bool IsFirstTimeLogIn()
        {
            throw new NotImplementedException();
        }
    }
}
