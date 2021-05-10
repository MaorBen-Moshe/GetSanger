using GetSanger.ViewModels;
using GetSanger.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.Registration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPersonalDetailPage : ContentPage
    {
        public SignupPersonalDetailPage()
        {
            InitializeComponent();

            BindingContext = AppManager.Instance.SignUpVM;
        }

        protected override bool OnBackButtonPressed()
        {
            if ((BindingContext as SignUpPageViewModel).IsFacebookGamil)
            {
                (BindingContext as SignUpPageViewModel).BackButtonBehaviorCommand.Execute(null);
            }
            else
            {
                base.OnBackButtonPressed();
            }

            return true;
        }

        protected override void OnAppearing()
        {
            (BindingContext as SignUpPageViewModel).Appearing();
            (BindingContext as SignUpPageViewModel).InPersonalPage = true;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as SignUpPageViewModel).InPersonalPage = false;
            base.OnDisappearing();
        }
    }
}