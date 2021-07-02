using System;
using GetSanger.Extensions;
using Foundation;
using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using StoreKit;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(RateAppService))]
namespace GetSanger.iOS.Services
{
    public class RateAppService : IAppRating
    {
        public async void RateApp()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
                SKStoreReviewController.RequestReview();
            else
            {
                var storeUrl = "itms-apps://itunes.apple.com/app/YourAppId";
                var url = storeUrl + "?action=write-review";

                try
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
                }
                catch (Exception ex)
                {
                    string message = string.Format(@"{0}
{1}", ex.Message, "App store was unable to load :(");
                    await ex.LogAndDisplayError($"{nameof(RateAppService)}:ios:RateApp", "Error", message);
                }
            }
        }
    }
}