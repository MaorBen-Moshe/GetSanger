using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using GetSanger.Controls;
using GetSanger.Droid.Renderers;
using System.ComponentModel;
using Android.Content;

[assembly: ExportRenderer(typeof(FrameWithShadow), typeof(ShadowFrameRenderer))]
namespace GetSanger.Droid.Renderers
{
    public class ShadowFrameRenderer : FrameRenderer
    {
        public FrameWithShadow ElementV2 => Element as FrameWithShadow;

        public ShadowFrameRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FrameWithShadow.ElevationProperty.PropertyName)
            {
                UpdateBackground();
            }
            else if(e.PropertyName == FrameWithShadow.ZProperty.PropertyName)
            {
                UpdateBackground();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected void UpdateBackground(FrameWithShadow element)
        {
            if (element == null) return;
            if (element.HasShadow)
            {
                Elevation = element.Elevation;
                TranslationZ = 0.0f;
                SetZ(element.Z);
            }
        }

        protected override void UpdateBackground()
        {
            UpdateBackground(ElementV2);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            UpdateBackground(e.NewElement as FrameWithShadow);
        }
    }
}