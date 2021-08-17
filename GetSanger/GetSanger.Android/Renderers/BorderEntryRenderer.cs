using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using GetSanger.Controls;
using GetSanger.Droid.Renderers;
using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(EntryWithBorder), typeof(BorderEntryRenderer))]
namespace GetSanger.Droid.Renderers
{
    public class BorderEntryRenderer : EntryRenderer
    {
        public EntryWithBorder ElementV2 => Element as EntryWithBorder;

        public BorderEntryRenderer(Context context) : base(context)
        {
        }

        protected override FormsEditText CreateNativeControl()
        {
            FormsEditText control = base.CreateNativeControl();
            UpdateBackground(control);
            return control;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == EntryWithBorder.CornerRadiusProperty.PropertyName)
            {
                UpdateBackground();
            }else if (e.PropertyName == EntryWithBorder.BorderThicknessProperty.PropertyName)
            {
                UpdateBackground();
            }
            else if (e.PropertyName == EntryWithBorder.BorderColorProperty.PropertyName)
            {
                UpdateBackground();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void UpdateBackgroundColor()
        {
            UpdateBackground();
        }

        protected void UpdateBackground(FormsEditText control)
        {
            if (control == null) return;

            var gd = new GradientDrawable();
            gd.SetColor(Element.BackgroundColor.ToAndroid());
            gd.SetCornerRadius(Context.ToPixels(ElementV2.CornerRadius));
            gd.SetStroke((int)Context.ToPixels(ElementV2.BorderThickness), ElementV2.BorderColor.ToAndroid());
            control.SetBackground(gd);

            var padTop = (int)Context.ToPixels(ElementV2.Padding.Top);
            var padBottom = (int)Context.ToPixels(ElementV2.Padding.Bottom);
            var padLeft = (int)Context.ToPixels(ElementV2.Padding.Left);
            var padRight = (int)Context.ToPixels(ElementV2.Padding.Right);

            control.SetPadding(padLeft, padTop, padRight, padBottom);
        }

        protected override void UpdateBackground()
        {
            UpdateBackground(Control);
        }
    }
}