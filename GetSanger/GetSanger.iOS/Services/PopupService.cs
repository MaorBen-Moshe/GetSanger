using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using GetSanger.Views;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(PopupService))]
namespace GetSanger.iOS.Services
{
    public class PopupService : IPopupService
    {
        private UIView _nativeView;
        private Stack<NativeViewPage> _contentPages = new Stack<NativeViewPage>();

        public Page CurrentShownPage
        {
            get
            {
                Page toRet = null;
                if (_contentPages.TryPeek(out NativeViewPage current))
                {
                    toRet = current.Page;
                }

                return toRet;
            }
        }

        public bool IsLoadingPageShowing
        {
            get
            {
                return CurrentShownPage != null && CurrentShownPage is LoadingPage;
            }
        }

        public void InitPopupgPage(ContentPage popupPage = null)
        {
            if (popupPage == null)
            {
                popupPage = new LoadingPage();
            }

            // build the loading page with native base
            Page current = _contentPages.Count == 0 ? Xamarin.Forms.Application.Current.MainPage : _contentPages.Peek().Page;
            popupPage.Parent = current;

            popupPage.Layout(new Rectangle(0, 0, current.Width, current.Height));

            var renderer = popupPage.GetOrCreateRenderer();

            _nativeView = renderer.NativeView;
            _contentPages.Push(new NativeViewPage { Page = popupPage, NativeView = _nativeView });
        }

        public void ShowPopupgPage()
        {
            // check if the user has set the page or not
            if (_contentPages.Count == 0)
                InitPopupgPage(new LoadingPage()); // set the default page

            // showing the native loading page
            UIApplication.SharedApplication.KeyWindow.AddSubview(_contentPages.Peek().NativeView);
        }

        public void HidePopupPage()
        {
            // Hide the page
            _contentPages.Pop().NativeView.RemoveFromSuperview();
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