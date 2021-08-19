using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutgoingViewCell : ViewCell
    {
        public OutgoingViewCell()
        {
            InitializeComponent();
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ForceUpdateSize();
        }
    }
}