using System.ComponentModel;
using System.Drawing;
using GetSanger.Controls;
using GetSanger.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EditorWithBorder), typeof(BorderEditorRenderer))]
namespace GetSanger.iOS.Renderers
{
    public class BorderEditorRenderer : EditorRenderer
    {
        public EditorWithBorder ElementV2 => Element as EditorWithBorder;

        public UITextViewPadding ControlV2 => Control as UITextViewPadding;

        protected void UpdateBackground(UITextView control)
        {
            if (control == null) return;
            control.Layer.CornerRadius = ElementV2.CornerRadius;
            control.Layer.BorderWidth = ElementV2.BorderThickness;
            control.Layer.BorderColor = ElementV2.BorderColor.ToCGColor();
        }

        protected override UITextView CreateNativeControl()
        {
            var control = new UITextViewPadding(RectangleF.Empty)
            {
                Padding = ElementV2.Padding,
                ClipsToBounds = true
            };

            UpdateBackground(control);

            return control;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorWithBorder.PaddingProperty.PropertyName)
            {
                UpdatePadding();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected void UpdatePadding()
        {
            if (Control == null)
                return;

            ControlV2.Padding = ElementV2.Padding;
        }
    }
}