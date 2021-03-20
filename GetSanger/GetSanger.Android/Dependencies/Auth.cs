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
using GetSanger.Droid.Dependencies;
using Xamarin.Forms;

[assembly: Dependency(typeof(Auth))]
namespace GetSanger.Droid.Dependencies
{
    class Auth : IAuth
    {
        public async Task<string> GetIdToken()
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;

            if (user == null)
            { 
                await FirebaseAuth.Instance.SignInAnonymouslyAsync();
                user = FirebaseAuth.Instance.CurrentUser;
            }

            return user.GetIdToken(true).Result.ToString();
        }

        public Task<string> GetUserId()
        {
            throw new NotImplementedException();
        }
    }
}