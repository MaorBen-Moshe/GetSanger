using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            if (user == null)
            {
                await Auth.DefaultInstance.SignInAnonymouslyAsync();
                user = getUser();
            }

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
    }
}