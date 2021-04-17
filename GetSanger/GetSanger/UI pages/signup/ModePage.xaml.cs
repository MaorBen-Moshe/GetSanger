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

        protected override bool OnBackButtonPressed()
        {
            DisplayAlert("Note", "You must choose a mode to continue.", "OK");
            return true;
        }
    }
}