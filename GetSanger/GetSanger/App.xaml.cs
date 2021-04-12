using Xamarin.Forms;
using GetSanger.AppShell;
using GetSanger.UI_pages.signup;
using GetSanger.UI_pages.common;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new SignupCategoriesPage();
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
