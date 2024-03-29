﻿using GetSanger.Interfaces;
using GetSanger.Services;
using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModePage : PopupPage
    {
        public ModePage()
        {
            InitializeComponent();

            Background = null;
        }

        protected override bool OnBackButtonPressed()
        {
            (BindingContext as ModeViewModel).SetBackBehavior();
            return base.OnBackButtonPressed();
        }

        protected override bool OnBackgroundClicked()
        {
            (BindingContext as ModeViewModel).SetBackBehavior();
            return base.OnBackgroundClicked();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IPageService service = AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
            service.DisplayAlert("Note", "You must choose a mode to continue.", "OK");
            (BindingContext as BaseViewModel).Appearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as BaseViewModel).Disappearing();
        }
    }
}