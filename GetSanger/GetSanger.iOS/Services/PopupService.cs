using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using GetSanger.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(PopupService))]
namespace GetSanger.iOS.Services
{
    public class PopupService : IPopupService
    {
        private UIView _nativeView;

        private bool _isInitialized;

        public void InitPopupgPage(ContentPage popupPage = null)
        {
            if(popupPage == null)
            {
                popupPage = new LoadingPage();
            }

            // build the loading page with native base
            popupPage.Parent = Xamarin.Forms.Application.Current.MainPage;

            popupPage.Layout(new Rectangle(0, 0,
                Xamarin.Forms.Application.Current.MainPage.Width,
                Xamarin.Forms.Application.Current.MainPage.Height));

            var renderer = popupPage.GetOrCreateRenderer();

            _nativeView = renderer.NativeView;

            _isInitialized = true;
        }

        public void ShowPopupgPage()
        {
            // check if the user has set the page or not
            if (!_isInitialized)
                InitPopupgPage(new LoadingPage()); // set the default page

            // showing the native loading page
            UIApplication.SharedApplication.KeyWindow.AddSubview(_nativeView);
        }

        public void HidePopupPage()
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