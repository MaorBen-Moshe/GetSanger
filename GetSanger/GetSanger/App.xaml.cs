using Xamarin.Forms;
using GetSanger.Views;
using GetSanger.AppShell;
using GetSanger.Views.Registration;
using Xamarin.Essentials;
using GetSanger.Interfaces;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // MainPage = new AuthShell(); // need to check first if the user is already connected and what mode the user is in.
            MainPage = new AddRatingPage();
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
            ILoadingService service = DependencyService.Get<ILoadingService>();
            if (e.NetworkAccess.Equals(NetworkAccess.None))
            {
                service.InitLoadingPage(new LoadingPage("No Internet"));
                service.ShowLoadingPage();
            }
            else
            {
                service.HideLoadingPage();
            }
        }
    }
}
