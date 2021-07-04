﻿using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutUsPage : PopupPage
    {
        public AboutUsPage()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}