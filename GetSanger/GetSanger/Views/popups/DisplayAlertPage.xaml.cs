using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayAlertPage : PopupPage
    {
        public DisplayAlertPage(string header, 
                         string message,
                         string okText = "OK",
                         string cancelText = null,
                         Action<bool> action = null)
        {
            InitializeComponent();

            BindingContext = new DisplayAlertViewModel(header, message, okText, cancelText, action);
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