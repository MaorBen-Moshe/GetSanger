using Xamarin.Forms;
using Xamarin.Essentials;
using GetSanger.Views;
using GetSanger.Services;

namespace GetSanger
{
    public partial class App : Application
    {
        private bool m_hasInternet;

        public App()
        {
            InitializeComponent();
            MainPage = new OnBoardingView();
            //VersionTracking.Track();
            //if (VersionTracking.IsFirstLaunchEver)
            //{
            //    MainPage = new OnBoardingView();
            //}
            //else
            //{
            //    MainPage = new SplashPage();
            //}
        }

        protected override void OnStart()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            base.OnStart();
        }

        protected override void OnSleep()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            base.OnSleep();
        }

        protected override void OnResume()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            base.OnResume();
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