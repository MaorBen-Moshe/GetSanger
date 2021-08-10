using GetSanger.Extensions;
using GetSanger.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GetSanger.ViewModels
{
    public class SplashViewModel : BaseViewModel
    {
        private bool m_ConnectivtyChanged = false;

        public SplashViewModel()
        {
        }

        public async override void Appearing()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            try
            {
                if (Connectivity.NetworkAccess.Equals(NetworkAccess.None) == false)
                {
                    await login();
                }
                else
                {
                    await sr_PageService.DisplayAlert("ERROR", "No Connection.", "Try again");
                }
            }
            catch (Exception e)
            {
                string message = string.Format("{0}\nPlease try to reopen the app!", e.Message);
                await e.LogAndDisplayError($"{nameof(SplashViewModel)}:Appearing", "Error", message, i_IsAcceptDisplay: false);
            }
        }

        public override void Disappearing()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        protected override void SetCommands()
        {
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (m_ConnectivtyChanged == false && e.NetworkAccess.Equals(NetworkAccess.Internet) == true)
            {
                m_ConnectivtyChanged = true;
                await login();
            }
        }

        private Task login()
        {
            AppManager.Instance.Services.SetDependencies();
            return sr_LoginServices.TryAutoLogin();
        }
    }
}