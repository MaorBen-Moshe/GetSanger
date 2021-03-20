using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace GetSanger.Droid.Services
{
    public abstract class DialogService
    {
        protected Android.Views.View _nativeView;

        protected Dialog _dialog;

        protected bool _isInitialized;

        public void InitDialogPage(ContentPage i_DialogIndicatorPage)
        {
            if (_isInitialized == false)
            {
                // check if the page parameter is available
                if (i_DialogIndicatorPage != null)
                {
                    // build the loading page with native base
                    i_DialogIndicatorPage.Parent = Xamarin.Forms.Application.Current.MainPage;

                    i_DialogIndicatorPage.Layout(new Rectangle(0, 0,
                        Xamarin.Forms.Application.Current.MainPage.Width,
                        Xamarin.Forms.Application.Current.MainPage.Height));

                    var renderer = i_DialogIndicatorPage.GetOrCreateRenderer();

                    _nativeView = renderer.View;

                    _dialog = new Dialog(CrossCurrentActivity.Current.Activity);
                    _dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
                    _dialog.SetCancelable(false);
                    _dialog.SetContentView(_nativeView);
                    Window window = _dialog.Window;
                    window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                    //window.ClearFlags(WindowManagerFlags.DimBehind);
                    window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

                    _isInitialized = true;
                }
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
                renderer = Platform.CreateRendererWithContext(bindable, null);
                Platform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }
    }
}