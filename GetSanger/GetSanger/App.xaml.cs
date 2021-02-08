using System;
using Xamarin.Forms;
using GetSangerUI;
using Xamarin.Forms.Xaml;
using GetSanger.UI_pages.signup;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new SignupPersonalDetailPage();
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
