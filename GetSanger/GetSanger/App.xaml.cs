using Xamarin.Forms;
using GetSanger.AppShell;
using Xamarin.Essentials;
using GetSanger.Interfaces;
using GetSanger.Views;
using GetSanger.Views.chat;
using GetSanger.Services;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

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
            IPopupService service = DependencyService.Get<IPopupService>();
            if (e.NetworkAccess.Equals(NetworkAccess.None))
            {
                service.InitPopupgPage(new LoadingPage("No Internet"));
                service.ShowPopupgPage();
            }
            else // Internet is back
            {
                if(service.CurrentShownPage is LoadingPage)
                {
                    service.HidePopupPage();
                }
            }
        }
    }
}
