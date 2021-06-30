using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddRatingPage : PopupPage
    {
        public AddRatingPage(string i_RatedUserId)
        {
            InitializeComponent();

            (BindingContext as AddRatingViewModel).RatedUserId = i_RatedUserId;
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }
    }
}