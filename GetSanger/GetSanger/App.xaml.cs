using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.Views.Forms;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new LoginWithSocialIconPage();
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
