using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using GetSanger.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(LoadingServices))]
namespace GetSanger.iOS.Services
{
    public class LoadingServices : ILoadingService
    {
        private UIView _nativeView;
        private bool _isInitialized = false;
        private bool _isLoading = false;

        public bool IsLoading => _isLoading;

        public void InitLoadingPage(ContentPage i_Page = null)
        {
            if (i_Page == null)
            {
                i_Page = new LoadingPage();
            }

            // build the loading page with native base
            Page current = Xamarin.Forms.Application.Current.MainPage;
            i_Page.Parent = current;

            i_Page.Layout(new Rectangle(0, 0, current.Width, current.Height));

            var renderer = i_Page.GetOrCreateRenderer();

            _nativeView = renderer.NativeView;
            _isInitialized = true;
        }

        public void ShowLoadingPage()
        {
            // check if the user has set the page or not
            if(_isLoading == false)
            {
                if (!_isInitialized)
                    InitLoadingPage(new LoadingPage()); // set the default page

                // showing the native loading page
                UIApplication.SharedApplication.KeyWindow.AddSubview(_nativeView);
                _isLoading = true;
            }
        }

        public void HideLoadingPage()
        {
            if (_isInitialized && _isLoading)
            {
                // Hide the page
                _nativeView.RemoveFromSuperview();
                _isLoading = false;
                _isInitialized = false;
            }
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

    public class NativeViewPage
    {
        public ContentPage Page { get; set; }

        public UIView NativeView { get; set; }
    }
}