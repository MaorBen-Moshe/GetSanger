using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingViewCell : ViewCell
    {
        public IncomingViewCell()
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