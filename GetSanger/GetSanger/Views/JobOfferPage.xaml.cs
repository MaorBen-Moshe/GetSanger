using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.ViewModels;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobOfferPage : ContentPage
    {
        public JobOfferPage()
        {
            InitializeComponent();

            BindingContext = new JobOfferViewModel();
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as JobOfferViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}