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
            }
            catch(Exception e)
            {
                // log errors
            }
        }

        protected override void OnDisappearing()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            base.OnDisappearing();
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if(e.NetworkAccess.Equals(NetworkAccess.None) == false)
            {
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