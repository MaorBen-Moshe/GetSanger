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
using Android.Content.PM;

namespace GetSanger.Droid.Services
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] {Android.Content.Intent.ActionView},
        Categories = new[] {Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable},
        DataScheme = "com.companyname.getsanger")]
    public class WebAuthenticationCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
    {
    }
}