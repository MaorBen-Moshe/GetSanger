using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using GetSanger.Views;
using Plugin.CurrentActivity;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace GetSanger.Droid.Services
{
    public abstract class DialogService
    {
        protected Android.Views.View _nativeView;
        protected Stack<PageDialog> _contentPages = new Stack<PageDialog>();
        protected Dialog _dialog;


        public void InitDialogPage(ContentPage i_DialogIndicatorPage)
        {
            // check if the page parameter is available
            if (i_DialogIndicatorPage != null)
            {
                // build the loading page with native base
                Page current = _contentPages.Count == 0 ? Xamarin.Forms.Application.Current.MainPage : _contentPages.Peek().Page;
                i_DialogIndicatorPage.Parent = current;
                i_DialogIndicatorPage.Layout(new Rectangle(0, 0, current.Width, current.Height));
                var renderer = i_DialogIndicatorPage.GetOrCreateRenderer();

                _nativeView = renderer.View;

                _dialog = new Dialog(CrossCurrentActivity.Current.Activity);
                _dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
                _dialog.SetCancelable(false);
                _dialog.SetContentView(_nativeView);
                Window window = _dialog.Window;
                window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                window.ClearFlags(WindowManagerFlags.DimBehind);
                //Android.Graphics.Color color = i_DialogIndicatorPage is LoadingPage ? Android.Graphics.Color.Transparent : Android.Graphics.Color.White;
                window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                _contentPages.Push(new PageDialog
                {
                    Page = i_DialogIndicatorPage,
                    CurrentDialog = _dialog
                });
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

    public class PageDialog
    {
        public ContentPage Page { get; set; }

        public Dialog CurrentDialog { get; set; }

        public bool IsLoading { get; set; }
    }
}