using GetSanger.Extensions;
using Android.Content;
using Android.Content.PM;
using GetSanger.Interfaces;
using GetSanger.Droid.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(RateAppService))]
namespace GetSanger.Droid.Services
{
        public class RateAppService : IAppRating
        {
            public async void RateApp()
            {
                var activity = Android.App.Application.Context;
                var url = $"market://details?id={(activity as Context)?.PackageName}";

                try
                {
                    activity.PackageManager.GetPackageInfo("com.android.vending", PackageInfoFlags.Activities);
                    Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                    
                    //this line is a problem
                    activity.StartActivity(intent);
                }
                catch (PackageManager.NameNotFoundException ex)
                {
                    await ex.LogAndDisplayError($"{nameof(RateAppService)}:android:RateApp", "Error", ex.Message);
                }
                catch (ActivityNotFoundException e)
                {
                // if Google Play fails to load, open the App link on the browser 
                await e.LogAndDisplayError($"{nameof(RateAppService)}:android:RateApp", "Error", e.Message);
                var playStoreUrl = "https://play.google.com/store/apps/details?id=com.yourapplicationpackagename"; //Add here the url of your application on the store

                    var browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(playStoreUrl));
                    browserIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);

                    activity.StartActivity(browserIntent);
                }
            }
    }
}