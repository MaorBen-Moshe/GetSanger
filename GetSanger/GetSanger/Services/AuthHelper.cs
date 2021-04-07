﻿using System;
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
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
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

        public static void ForgotPassword(string i_Email)
        {
            throw new NotImplementedException();
        }
    }
}
