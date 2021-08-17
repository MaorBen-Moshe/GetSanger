using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GetSanger.Exceptions;
using GetSanger.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public static class AuthHelper
    {
        private static readonly IAuth sr_Auth;

        static AuthHelper()
        {
            sr_Auth = DependencyService.Get<IAuth>();
        }

        public static async Task RegisterViaEmail(string i_Email, string i_Password)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (!sr_Auth.IsAnonymousUser())
                {
                    sr_Auth.SignOut();
                    await sr_Auth.SignInAnonymouslyAsync();
                }

                Dictionary<string, string> details = new Dictionary<string, string>()
                {
                    ["email"] = i_Email,
                    ["password"] = i_Password
                };

                string idToken = await GetIdTokenAsync();
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/RegisterUserWithEmailAndPassword";
                string json = ObjectJsonSerializer.SerializeForServer(details);
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    string responseMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception(responseMessage);
                }

                await LoginViaEmail(i_Email, i_Password);
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static void SignOut()
        {
            sr_Auth.SignOut();
        }

        public static async Task SendVerificationEmail()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SendVerificationEmail";
                string idToken = await GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, "", HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    string responseMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception(responseMessage);
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task LoginViaEmail(string i_Email, string i_Password)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SignInWithPassword";
                Dictionary<string, string> details = new Dictionary<string, string>()
                {
                    ["Email"] = i_Email,
                    ["Password"] = i_Password
                };

                string json = ObjectJsonSerializer.SerializeForServer(details);
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
                if (!response.IsSuccessStatusCode)
                {
                    string responseMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception(responseMessage);
                }

                string customToken = await response.Content.ReadAsStringAsync();
                SignOut();
                await sr_Auth.SignInWithCustomToken(customToken);
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task LinkWithEmailAndPassword(string i_Email, string i_Password)
        {
            Dictionary<string, string> requestDictionary = new Dictionary<string, string>()
            {
                ["idToken"] = await GetIdTokenAsync(),
                ["email"] = i_Email,
                ["password"] = i_Password
            };

            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/LinkWithEmailAndPassword";
            string json = ObjectJsonSerializer.SerializeForServer(requestDictionary);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            string responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(responseString);
            }
        }

        public static async Task<Dictionary<string, object>> LoginWithProvider(eSocialProvider i_Provider)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Dictionary<string, string> requestDictionary = new Dictionary<string, string>();
                switch (i_Provider)
                {
                    case eSocialProvider.Facebook:
                        //await sr_Auth.LoginViaFacebook();
                        //string facebookAccessToken = sr_Auth.GetFacebookAccessToken();
                        string facebookAccessToken = await getSocialAuthIdToken("Facebook");
                        requestDictionary["postBody"] = $"access_token={facebookAccessToken}&providerId=facebook.com";
                        requestDictionary["ProviderId"] = "facebook.com";
                        break;

                    case eSocialProvider.Google:
                        string googleIdToken = await getSocialAuthIdToken("Google");
                        requestDictionary["postBody"] = $"id_token={googleIdToken}&providerId=google.com";
                        requestDictionary["ProviderId"] = "google.com";
                        break;

                    case eSocialProvider.Apple:
                        string appleIdToken = await getSocialAuthIdToken("Apple");
                        requestDictionary["postBody"] = $"id_token={appleIdToken}&providerId=apple.com";
                        requestDictionary["ProviderId"] = "apple.com";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(i_Provider), i_Provider, null);
                }

                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SignInWithCredential";
                string json = ObjectJsonSerializer.SerializeForServer(requestDictionary);
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Dictionary<string, object> responseDictionary =
                        ObjectJsonSerializer.DeserializeForAuth(responseString) as Dictionary<string, object>;
                    string customToken = responseDictionary["customToken"] as string;
                    sr_Auth.SignOut();
                    await sr_Auth.SignInWithCustomToken(customToken);
                    return responseDictionary;
                }
                else
                {
                    throw new Exception(responseString);
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task<Dictionary<string, object>> LinkWithSocialProvider(eSocialProvider i_Provider)
        {
            string id = await GetIdTokenAsync();
            Dictionary<string, string> requestDictionary = new Dictionary<string, string>()
            {
                ["idToken"] = id
            };

            switch (i_Provider)
            {
                case eSocialProvider.Facebook:
                    string facebookAccessToken = await getSocialAuthIdToken("Facebook");
                    requestDictionary["postBody"] = $"access_token={facebookAccessToken}&providerId=facebook.com";
                    requestDictionary["ProviderId"] = "facebook.com";
                    break;

                case eSocialProvider.Google:
                    string googleIdToken = await getSocialAuthIdToken("Google");
                    requestDictionary["postBody"] = $"id_token={googleIdToken}&providerId=google.com";
                    requestDictionary["ProviderId"] = "google.com";
                    break;

                case eSocialProvider.Apple:
                    string appleIdToken = await getSocialAuthIdToken("Apple");
                    requestDictionary["postBody"] = $"id_token={appleIdToken}&providerId=apple.com";
                    requestDictionary["ProviderId"] = "apple.com";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(i_Provider), i_Provider, null);
            }

            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/LinkWithCredential";
            string json = ObjectJsonSerializer.SerializeForServer(requestDictionary);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            string responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                Dictionary<string, object> responseDictionary = ObjectJsonSerializer.DeserializeForAuth(responseString) as Dictionary<string, object>;
                return responseDictionary;
            }
            else
            {
                throw new Exception(responseString);
            }
        }

        private static async Task<string> getSocialAuthIdToken(string i_Scheme)
        {
            try
            {
                WebAuthenticatorResult webResult = null;
                string idToken = null;
                if (i_Scheme.Equals("Apple")
                    && DeviceInfo.Platform == DevicePlatform.iOS
                    && DeviceInfo.Version.Major >= 13)
                {
                    // Use Native Apple Sign In API's
                    var options = new AppleSignInAuthenticator.Options
                    {
                        IncludeEmailScope = true,
                        IncludeFullNameScope = true,
                    };
                    webResult = await AppleSignInAuthenticator.AuthenticateAsync(options);
                    idToken = webResult.IdToken;
                }
                else
                {
                    string uriString = "", callBackUrl = "com.companyname.getsanger://";

                    if (i_Scheme.Equals("Google"))
                    {
                        string clientId = "889764842790-4sni1ulkm38maitr4h1ibqamchcv8qkl.apps.googleusercontent.com";
                        string url = "https://europe-west3-get-sanger.cloudfunctions.net/SignInWithGoogle";
                        uriString =
                            $"https://accounts.google.com/o/oauth2/v2/auth?scope=openid profile email&response_type=code&redirect_uri={url}&client_id={clientId}&prompt=select_account";
                        webResult = await WebAuthenticator.AuthenticateAsync(new Uri(uriString), new Uri(callBackUrl));
                        idToken = webResult.IdToken;
                    }
                    else if (i_Scheme.Equals("Facebook"))
                    {
                        string clientId = "328227848585394";
                        string url = "https://europe-west3-get-sanger.cloudfunctions.net/SignInWithFacebook";
                        uriString =
                            $"https://www.facebook.com/v10.0/dialog/oauth?client_id={clientId}&redirect_uri={url}&scope=email&auth_type=rerequest,reauthenticate";
                        webResult = await WebAuthenticator.AuthenticateAsync(new Uri(uriString), new Uri(callBackUrl));
                        idToken = webResult.AccessToken;
                    }
                    else if (i_Scheme.Equals("Apple"))
                    {
                        string clientId = "com.signin.getsanger";
                        string url = "https://europe-west3-get-sanger.cloudfunctions.net/SignInWithApple";
                        uriString =
                            $"https://appleid.apple.com/auth/authorize?client_id={clientId}&redirect_uri={url}&response_type=code id_token&scope=name email&response_mode=form_post";
                        webResult = await WebAuthenticator.AuthenticateAsync(new Uri(uriString), new Uri(callBackUrl));
                        idToken = webResult.IdToken;
                    }
                }

                return idToken;
            }
            catch (OperationCanceledException)
            {
                throw new Exception("Login canceled.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed: {ex.Message}");
            }
        }

        public static bool IsLoggedIn()
        {
            try
            {
                return sr_Auth.IsLoggedIn();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> IsVerifiedEmail()
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/IsEmailVerified";
            string idToken = await GetIdTokenAsync();
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, "", HttpMethod.Post, idToken);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                bool result = ObjectJsonSerializer.DeserializeForServer<bool>(responseBody);
                return result;
            }
            else
            {
                throw new Exception(responseBody);
            }
        }

        public static string GetLoggedInUserId()
        {
            return sr_Auth.GetUserId();
        }

        public static async Task<bool> IsFirstTimeLogIn()
        {
            string uid = GetLoggedInUserId();

            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                ["userId"] = uid
            };
            string json = ObjectJsonSerializer.SerializeForServer(dictionary);
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/IsUserInDatabase";
            string idToken = await GetIdTokenAsync();
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            string responseMessage = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                bool result = ObjectJsonSerializer.DeserializeForServer<bool>(responseMessage);
                return !result;
            }
            else
            {
                throw new Exception(responseMessage);
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
                i_Verify = Regex.Replace(i_Verify, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                static string DomainMapper(Match match)
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
                return Regex.IsMatch(i_Verify, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static async Task ForgotPassword(string i_Email)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>()
                {
                    ["email"] = i_Email
                };
                string json = ObjectJsonSerializer.SerializeForServer(dictionary);
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/SendPasswordResetEmail";
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task<string> GetIdTokenAsync()
        {
            return await sr_Auth.GetIdToken();
        }

        public static async Task<bool> IsUserPassword(string i_Password)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/IsUserPassword";
                Dictionary<string, string> details = new Dictionary<string, string>()
                {
                    ["Password"] = i_Password
                };

                string json = ObjectJsonSerializer.SerializeForServer(details);
                string idToken = await GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                string responseMessage = await response.Content.ReadAsStringAsync();
                bool result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(responseMessage);
                }
                else
                {
                    result = ObjectJsonSerializer.DeserializeForServer<bool>(responseMessage);
                }

                return result;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task ChangePassword(string i_OldPassword, string i_NewPassword)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/ChangeUserPassword";
                Dictionary<string, string> details = new Dictionary<string, string>()
                {
                    ["OldPassword"] = i_OldPassword,
                    ["NewPassword"] = i_NewPassword
                };

                string json = ObjectJsonSerializer.SerializeForServer(details);
                string idToken = await GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }
    }
}