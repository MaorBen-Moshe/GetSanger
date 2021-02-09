using System;
using Xamarin.Forms;
using GetSangerUI;
using Xamarin.Forms.Xaml;
using GetSanger.UI_pages.signup;
using GetSanger.UI_pages;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AccountPage();
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
