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

            Android.Gms.Tasks.Task getIdTokenTask = user.GetIdToken(true);
            await getIdTokenTask;

            return user.Zze();
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
            if (getUser() != null)
            {
                FirebaseAuth.Instance.SignOut();
            }
        }

        public bool IsLoggedIn()
        {
            return getUser() != null && !IsAnonymousUser();
        }

        public async Task SignInWithCustomToken(string i_Token)
        {
            await FirebaseAuth.Instance.SignInWithCustomTokenAsync(i_Token);
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

        public string GetGoogleClientId()
        {
            return "889764842790-5125j7nj3p06j7ffivd4765ctvht9cnk.apps.googleusercontent.com";
        }

        private FirebaseUser getUser()
        {
            return FirebaseAuth.Instance.CurrentUser;
        }
    }
}