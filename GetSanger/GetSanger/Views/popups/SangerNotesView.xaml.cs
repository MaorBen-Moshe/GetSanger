using GetSanger.Models;
using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SangerNotesView : PopupPage
    {
        public SangerNotesView(JobOffer i_JobOffer)
        {
            BindingContext = new SangerNotesViewModel(i_JobOffer);

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BaseViewModel).Appearing();
        }
    }
}