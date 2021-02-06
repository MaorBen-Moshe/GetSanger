using System;
using Xamarin.Forms;
using GetSangerUI;
using Xamarin.Forms.Xaml;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Registration();
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
