using Xamarin.Forms;
using Xamarin.Essentials;
using GetSanger.Interfaces;
using GetSanger.Views;
using GetSanger.Services;
using System;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            AuthHelper.SignOut();

            LoginServices login = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            login.TryAutoLogin();
        }

        protected override void OnStart()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected override void OnSleep()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        protected override void OnResume()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            PopupService service = AppManager.Instance.Services.GetService(typeof(PopupService)) as PopupService;
            if (e.NetworkAccess.Equals(NetworkAccess.None))
            {
                service.ShowPopup(new LoadingPage("No Internet"));
            }
            else // Internet is back
            {
                service.HidePopup();
            }
        }
    }
}
