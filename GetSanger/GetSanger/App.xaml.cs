using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.Views.Forms;
using GetSanger.UI_pages.signup;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new RegistrationPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
