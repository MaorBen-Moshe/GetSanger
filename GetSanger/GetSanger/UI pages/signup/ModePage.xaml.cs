using GetSanger.UI_pages.sanger;
using GetSanger.UI_pages.user;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.UI_pages.signup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModePage : ContentPage
    {
        public ModePage()
        {
            InitializeComponent();
        }

        private void UserButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new UserMainPage();
        }

        private void SangerButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new SangerMainPage();
        }
    }
}