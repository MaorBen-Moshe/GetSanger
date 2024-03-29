﻿using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermsOfServicePage : PopupPage
    {
        public TermsOfServicePage()
        {
            InitializeComponent();

            Background = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as PopupBaseViewModel).Appearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as PopupBaseViewModel).Disappearing();
        }
    }
}