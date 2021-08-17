using Foundation;
using System;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Firebase.Auth;
using GetSanger.Interfaces;
using GetSanger.iOS.Services;
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

        public async Task LoginViaFacebook()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;

            LoginManager manager = new LoginManager();
            manager.LogOut();
            await Task.Run(() =>
            {
                manager.LogIn(new[] { "public_profile", "email" }, vc, ((result, error) =>
                {
                    if (error != null || result == null || result.IsCancelled)
                    {
                        throw new Exception("Login cancelled.");
                    }
                }));
            });
        }

        public string GetFacebookAccessToken()
        {
            return AccessToken.CurrentAccessToken.TokenString;
        }
    }
}