using Xamarin.Forms;
using Xamarin.Essentials;
using GetSanger.Views;
using GetSanger.Interfaces;
using GetSanger.Services;

namespace GetSanger
{
    public partial class App : Application
    {
        private bool m_hasInternet;

        public App()
        {
            InitializeComponent();
            if(Properties.ContainsKey(Constants.Constants.StartProperty) == false)
            {
                MainPage = new OnBoardingView();
            }
            else
            {
                MainPage = new SplashPage();
            }
        }

        protected override void OnStart()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            base.OnStart();
        }

        protected async override void OnSleep()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            ITrip trip = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            bool shared = await trip.TryShareSangerLoaction();
            if (shared)
            {
                trip.LeaveTripThread();
            }

            base.OnSleep();
        }

        protected async override void OnResume()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            ITrip trip = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            bool shared = await trip.TryShareSangerLoaction();
            if (shared)
            {
                trip.StartTripThread();
            }
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