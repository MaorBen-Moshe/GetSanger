using GetSanger.Services;
using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.Registration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupCategoriesPage : ContentPage
    {
        public SignupCategoriesPage()
        {
            InitializeComponent();

            //BindingContext = AppManager.Instance.SignUpVM;
            
            BindingContext = new SignUpPageViewModel();
        }
    }
}