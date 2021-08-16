using GetSanger.Controls;
using GetSanger.iOS.Rendrers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderWithEntry), typeof(BorderEntryRenderer))]
namespace GetSanger.iOS.Rendrers
{
    public class BorderEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // do whatever you want to the UITextField here!
                Control.BackgroundColor = UIColor.White;
                Control.BorderStyle = UITextBorderStyle.RoundedRect;
            }
        }
    }
}