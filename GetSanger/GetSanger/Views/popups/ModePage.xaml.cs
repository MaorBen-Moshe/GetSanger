using GetSanger.Services;
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
            setBackBehavior();
            return base.OnBackButtonPressed();
        }

        protected override bool OnBackgroundClicked()
        {
            setBackBehavior();
            return base.OnBackgroundClicked();
        }

        protected override void OnAppearing()
        {
            DisplayAlert("Note", "You must choose a mode to continue.", "OK");
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }

        private void setBackBehavior()
        {
            if (AuthHelper.IsLoggedIn())
            {
                AuthHelper.SignOut();
            }
        }
    }
}