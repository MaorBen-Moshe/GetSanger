﻿using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewJobOfferPage : ContentPage
    {
        public ViewJobOfferPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BaseViewModel).Appearing();
            if (!(BindingContext as ViewJobOfferViewModel).IsDeliveryCategory)
            {
                mainStack.Children.Remove(fromStack);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as BaseViewModel).Disappearing();
        }
    }
}