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
            (BindingContext as SignUpPageViewModel).BackButtonBehaviorCommand.Execute(null);
            return true;
        }
    }
}