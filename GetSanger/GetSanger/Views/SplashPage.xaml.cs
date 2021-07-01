using GetSanger.Extensions;
using GetSanger.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        private bool m_ConnectivtyChanged = false;

        public SplashPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            base.OnAppearing();
            try
            {
                if(Connectivity.NetworkAccess.Equals(NetworkAccess.None) == false)
                {
                    await login();
                }
                else
                {
                    await DisplayAlert("ERROR", "No Connection.", "Try again");
                }
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError(nameof(SplashPage), "Error", e.Message);
            }
        }

        protected override void OnDisappearing()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            base.OnDisappearing();
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if(m_ConnectivtyChanged == false && e.NetworkAccess.Equals(NetworkAccess.Internet) == true)
            {
                m_ConnectivtyChanged = true;
                await login();
            }
        }

        private Task login()
        {
            AppManager.Instance.Services.SetDependencies();
            LoginServices login = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            return login.TryAutoLogin();
        }
    }
}