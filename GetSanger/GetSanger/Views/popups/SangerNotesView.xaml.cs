using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SangerNotesView : PopupPage
    {
        public SangerNotesView(JobOffersViewModel i_BindingContext)
        {
            BindingContext = i_BindingContext;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }
    }
}