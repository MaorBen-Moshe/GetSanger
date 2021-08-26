using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace GetSanger.iOS.Renderers
{
    public class UITextViewPadding : UITextView
    {
        private Thickness _padding = new Thickness(5);

        public Thickness Padding
        {
            get => _padding;
            set
            {
                if (_padding != value)
                {
                    _padding = value;
                    //InvalidateIntrinsicContentSize();
                }
            }
        }

        public UITextViewPadding()
        {
        }
        public UITextViewPadding(NSCoder coder) : base(coder)
        {
        }

        public UITextViewPadding(CGRect rect) : base(rect)
        {
        }
    }
}