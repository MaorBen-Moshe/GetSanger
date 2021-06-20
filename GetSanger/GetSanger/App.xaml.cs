using Xamarin.Forms;
using Xamarin.Essentials;
using GetSanger.Services;

namespace GetSanger
{
    public partial class App : Application
    {
        private bool m_hasInternet;

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

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            m_hasInternet = e.NetworkAccess.Equals(NetworkAccess.Internet);
            while (m_hasInternet == false)
            {
                await Current.MainPage.DisplayAlert("ERROR", "No Connection.", "Try again");
            }
        }
    }
}
