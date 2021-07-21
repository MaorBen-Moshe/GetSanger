using GetSanger.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as BaseViewModel).Disappearing();
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            (BindingContext as SettingViewModel).BackButtonCommand.Execute(null);
            return true;
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double stepValue = 1.0;
            double newStep = Math.Round(e.NewValue / stepValue);
            (sender as Slider).Value = newStep * stepValue;
        }
    }
}