using Android.Content;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using GetSanger.Controls;
using GetSanger.Droid.Rendrers;

[assembly: ExportRenderer(typeof(BorderWithEntry), typeof(BorderEntryRenderer))]
namespace GetSanger.Droid.Rendrers
{
    class BorderEntryRenderer : EntryRenderer
    {
        public BorderEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundColor(global::Android.Graphics.Color.LightGray);
            }
        }
    }
}