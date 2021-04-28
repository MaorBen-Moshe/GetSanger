using Xamarin.Forms;
using GetSanger.AppShell;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

           MainPage = new UserShell(); // need to check first if the user is already connected and what mode the user is in.
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
