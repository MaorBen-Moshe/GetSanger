using System;
using System.Collections.Generic;
using GetSanger.Interfaces;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Android.Runtime;
using Firebase.Auth;
using GetSanger.Droid.Services;
using Plugin.CurrentActivity;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Forms;

[assembly: Dependency(typeof(Auth))]

namespace GetSanger.Droid.Services
{
    class Auth : IAuth
    {
        internal static ICallbackManager FacebookCallbackManager { get; } = CallbackManagerFactory.Create();

        public async Task<string> GetIdToken()
        {
            FirebaseUser user = getUser();

            GetTokenResult getTokenResult = (GetTokenResult)await user.GetIdToken(true);
            string token = getTokenResult.Token;

            return token;
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

        public async Task SignInAnonymouslyAsync()
        {
            await FirebaseAuth.Instance.SignInAnonymouslyAsync();
        }

        public async Task LoginViaFacebook()
        {
            LoginManager.Instance.LogOut();
            LoginManager.Instance.RegisterCallback(FacebookCallbackManager, new FacebookCallback<LoginResult>()
            {
                HandleError = (exception => throw new Exception("Login failed.")),
                HandleCancel = (() => throw new Exception("Login cancelled.")),
                HandleSuccess = (result => AccessToken.CurrentAccessToken = result.AccessToken)
            });

            LoginManager.Instance.SetLoginBehavior(LoginBehavior.NativeWithFallback);
            //LoginManager.Instance.LogIn(CrossCurrentActivity.Current.Activity, new List<string> { "public_profile", "email" });
            await Task.Run(() => LoginManager.Instance.LogInWithReadPermissions(CrossCurrentActivity.Current.Activity,
                new List<string> { "public_profile", "email" }));
        }

        public string GetFacebookAccessToken()
        {
            //return AccessToken.CurrentAccessToken.Token;
            return "";
        }

        private FirebaseUser getUser()
        {
            return FirebaseAuth.Instance.CurrentUser;
        }
    }

    class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
    {
        public Action HandleCancel { get; set; }
        public Action<FacebookException> HandleError { get; set; }
        public Action<TResult> HandleSuccess { get; set; }

        public void OnCancel()
        {
            var c = HandleCancel;
            if (c != null)
                c();
        }

        public void OnError(FacebookException error)
        {
            var c = HandleError;
            if (c != null)
                c(error);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var c = HandleSuccess;
            if (c != null)
                c(result.JavaCast<TResult>());
        }
    }
}