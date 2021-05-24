using Xamarin.Forms;
using GetSanger.AppShell;
using Xamarin.Essentials;
using GetSanger.Interfaces;
using GetSanger.Views;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AuthShell(); // need to check first if the user is already connected and what mode the user is in.
           
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
            IPopupService service = DependencyService.Get<IPopupService>();
            if (e.NetworkAccess.Equals(NetworkAccess.None))
            {
                service.InitPopupgPage(new LoadingPage("No Internet"));
                service.ShowPopupgPage();
            }
            else
            {
                service.HidePopupPage();
            }
        }
    }
}
