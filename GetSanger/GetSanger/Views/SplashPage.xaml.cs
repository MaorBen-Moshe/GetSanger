using GetSanger.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                AppManager.Instance.Services.SetDependencies();
                LoginServices login = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
                await login.TryAutoLogin();
            }
            catch(Exception e)
            {
                // log errors
            }
        }
    }
}