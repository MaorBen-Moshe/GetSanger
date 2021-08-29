using Foundation;
using System;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Firebase.Auth;
using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using GetSanger.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthIOs))]

namespace GetSanger.iOS.Services
{
    class AuthIOs : IAuth
    {
        public async Task<string> GetIdToken()
        {
            User user = getUser();

            string idToken = await user.GetIdTokenAsync(true);

            return idToken;
        }

        private User getUser()
        {
            return Auth.DefaultInstance.CurrentUser;
        }

        public string GetUserId()
        {
            User user = getUser();
            string uid = null;

            if (user != null)
            {
                uid = user.Uid;
            }

            return uid;
        }

        public void SignOut()
        {
            if (getUser() != null)
            {
                NSError? error;
                Auth.DefaultInstance.SignOut(out error);
            }
        }

        public bool IsLoggedIn()
        {
            return getUser() != null && !IsAnonymousUser();
        }

        public async Task SignInWithCustomToken(string i_Token)
        {
            await Auth.DefaultInstance.SignInWithCustomTokenAsync(i_Token);
        }

        public bool IsAnonymousUser()
        {
            bool result = false;

            if (getUser() != null)
            {
                result = getUser().IsAnonymous;
            }

            return result;
        }

        public async Task SignInAnonymouslyAsync()
        {
            await Auth.DefaultInstance.SignInAnonymouslyAsync();
        }

        public void LoginViaFacebook()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;

            LoginManager manager = new LoginManager();
            manager.LogOut();
            manager.LogIn(new[] { "public_profile", "email" }, vc, ((result, error) =>
            {
                if (result is { IsCancelled: false })
                {
                    AccessToken.CurrentAccessToken = result.Token;
                }

                AuthHelper.FacebookLoginCompletion?.TrySetResult(true);
            }));
        }

        public string GetFacebookAccessToken()
        {
            try
            {
                string accessToken = AccessToken.CurrentAccessToken.TokenString;

                if (accessToken == null)
                {
                    throw new Exception();
                }

                return accessToken;
            }
            catch (Exception)
            {
                throw new Exception("Login failed.");
            }
        }
    }
}