using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using GetSanger.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(LoadingService))]
namespace GetSanger.iOS.Services
{
    public class LoadingService : ILoadingService
    {
        private UIView _nativeView;

        private bool _isInitialized;

        public void InitLoadingPage(ContentPage loadingIndicatorPage = null)
        {
            if(loadingIndicatorPage == null)
            {
                loadingIndicatorPage = new LoadingPage();
            }

            // build the loading page with native base
            loadingIndicatorPage.Parent = Xamarin.Forms.Application.Current.MainPage;

            loadingIndicatorPage.Layout(new Rectangle(0, 0,
                Xamarin.Forms.Application.Current.MainPage.Width,
                Xamarin.Forms.Application.Current.MainPage.Height));

            var renderer = loadingIndicatorPage.GetOrCreateRenderer();

            _nativeView = renderer.NativeView;

            _isInitialized = true;
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