using Xamarin.Forms;
using GetSanger.AppShell;
using GetSanger.Views;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AuthShell(); // need to check first if the user is already connected and what mode the user is in.
           // MainPage = new signupEmailPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
