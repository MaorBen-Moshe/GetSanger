using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetSanger.Services;
using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.Registration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupEmailPage : ContentPage
    {
        public SignupEmailPage()
        {
            InitializeComponent();

            BindingContext = AppManager.Instance.SignUpVM;
        }

        protected override bool OnBackButtonPressed()
        {
            (BindingContext as SignUpPageViewModel).BackButtonBehaviorCommand.Execute(null);
            return true;
        }

        protected override void OnAppearing()
        {
            (BindingContext as SignUpPageViewModel).Appearing();
            base.OnAppearing();
        }
    }
}