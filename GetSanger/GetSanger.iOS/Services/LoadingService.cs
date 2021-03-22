using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using GetSanger.UI_pages.common;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(LoadingService))]
namespace GetSanger.iOS.Services
{
    public class LoadingService : ILoadingService
    {
        private UIView _nativeView;

        private bool _isInitialized;

        public void InitLoadingPage(ContentPage i_LoadingIndicatorPage = null)
        {
            // check if the page parameter is available
            if (i_LoadingIndicatorPage != null)
            {
                // build the loading page with native base
                i_LoadingIndicatorPage.Parent = Xamarin.Forms.Application.Current.MainPage;

                i_LoadingIndicatorPage.Layout(new Xamarin.Forms.Rectangle(0, 0,
                    Xamarin.Forms.Application.Current.MainPage.Width,
                    Xamarin.Forms.Application.Current.MainPage.Height));

                var renderer = i_LoadingIndicatorPage.GetOrCreateRenderer();

                _nativeView = renderer.NativeView;

                _isInitialized = true;
            }
        }

        public void ShowLoadingPage()
        {
            // check if the user has set the page or not
            if (!_isInitialized)
                InitLoadingPage(new LoadingPage()); // set the default page

            // showing the native loading page
            UIApplication.SharedApplication.KeyWindow.AddSubview(_nativeView);
        }

        public void HideLoadingPage()
        {
            // Hide the page
            _nativeView.RemoveFromSuperview();
        }
    }

    internal static class PlatformExtension
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable)
        {
            var renderer = Platform.GetRenderer(bindable);
            if (renderer == null)
            {
                renderer = Platform.CreateRenderer(bindable);
                Platform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }
    }
}