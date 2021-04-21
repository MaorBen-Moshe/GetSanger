using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
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

            string idToken = await GetIdToken();
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

        public static Task<Dictionary<string, string>> LoginViaGoogle()
        {
            throw new NotImplementedException();
        }

        public static Task<Dictionary<string, string>> LoginViaFacebook()
        {
            throw new NotImplementedException();
        }

        public static bool IsLoggedIn()
        {
            return s_Auth.IsLoggedIn();
        }

        public static bool IsVerifiedEmail()
        {
            return true;
        }

        public static string GetLoggedInUserId()
        {
            return s_Auth.GetUserId();
        }

        public static async Task<bool> IsFirstTimeLogIn()
        {
            string uid = GetLoggedInUserId();

            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                ["userId"] = uid
            };
            string json = JsonSerializer.Serialize(dictionary);
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/IsUserInDatabase";
            string idToken = await GetIdToken();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);

            if (response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                bool result = JsonSerializer.Deserialize<bool>(responseMessage);
                return result;
            }
            else
            {
                throw new Exception("Error");
            }
        }

        public static bool IsValidEmail(string i_Verify)
        {
            if (string.IsNullOrWhiteSpace(i_Verify))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                i_Verify = Regex.Replace(i_Verify, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(i_Verify,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static async Task ForgotPassword(string i_Email)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                ["email"] = i_Email
            };
            string json = JsonSerializer.Serialize(dictionary);
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SendPasswordResetEmail";
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public static async Task<string> GetIdToken()
        {
            return await s_Auth.GetIdToken();
        }
    }
}