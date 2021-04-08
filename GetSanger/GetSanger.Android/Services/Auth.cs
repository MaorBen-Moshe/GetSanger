using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetSanger.Interfaces;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Firebase.Auth;
using GetSanger.Droid.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Auth))]
namespace GetSanger.Droid.Services
{
    class Auth : IAuth
    {
        public async Task<string> GetIdToken()
        {
            FirebaseUser user = getUser();

            if (user == null)
            {
                await FirebaseAuth.Instance.SignInAnonymouslyAsync();
                user = getUser();
            }

            await user.GetIdToken(true);
            return user.Zzh();
        }

        public string GetUserId()
        {
            FirebaseUser user = getUser();
            string uid = null;

            if (user != null)
            {
                uid = user.Uid;
            }

            return uid;
        }

        public void SignOut()
        {
            if (IsLoggedIn())
            {
                FirebaseAuth.Instance.SignOut();
            }
        }

        public bool IsLoggedIn()
        {
            return getUser() != null;
        }

        public void SignInWithCustomToken(string i_Token)
        {
            SignOut();
            FirebaseAuth.Instance.SignInWithCustomTokenAsync(i_Token);
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

        private FirebaseUser getUser()
        {
            return FirebaseAuth.Instance.CurrentUser;
        }
    }
}