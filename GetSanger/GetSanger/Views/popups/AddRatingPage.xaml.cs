using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddRatingPage : PopupPage
    {
        public AddRatingPage(string i_RatedUserId, string i_RatedUserName)
        {
            InitializeComponent();

            Background = null;
            (BindingContext as AddRatingViewModel).RatedUserId = i_RatedUserId;
            (BindingContext as AddRatingViewModel).UserName = i_RatedUserName;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BaseViewModel).Appearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as BaseViewModel).Disappearing();
        }
    }
}