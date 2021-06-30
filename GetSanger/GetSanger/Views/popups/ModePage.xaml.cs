using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModePage : PopupPage
    {
        public ModePage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            DisplayAlert("Note", "You must choose a mode to continue.", "OK");
            return true;
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }
    }
}