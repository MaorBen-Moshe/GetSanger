using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using GetSanger.Controls;
using GetSanger.Droid.Renderers;
using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;

[assembly: ExportRenderer(typeof(PickerWithBorder), typeof(BorderPickerRenderer))]
namespace GetSanger.Droid.Renderers
{
    public class BorderPickerRenderer : PickerRenderer
    {
        public PickerWithBorder ElementV2 => Element as PickerWithBorder;

        public BorderPickerRenderer(Context context) : base(context)
        {
        }

        protected override EditText CreateNativeControl()
        {
            EditText control = base.CreateNativeControl();
            UpdateBackground(control);
            return control;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PickerWithBorder.CornerRadiusProperty.PropertyName)
            {
                UpdateBackground();
            }
            else if (e.PropertyName == PickerWithBorder.BorderThicknessProperty.PropertyName)
            {
                UpdateBackground();
            }
            else if (e.PropertyName == PickerWithBorder.BorderColorProperty.PropertyName)
            {
                UpdateBackground();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void UpdateBackgroundColor()
        {
            UpdateBackground();
        }

        protected void UpdateBackground(EditText control)
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