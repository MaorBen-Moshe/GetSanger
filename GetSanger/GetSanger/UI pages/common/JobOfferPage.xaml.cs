using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.ViewModels;

namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobOfferPage : ContentPage
    {
        public JobOfferPage()
        {
            BindingContext = new JobOfferViewModel();

            InitializeComponent();
        }

        private async void JobLocationButton_Clicked(object sender, EventArgs e)
        {
            MapPage map = new MapPage();
            await Navigation.PushAsync(map);
        }

        private void CurrentLocationButton_Clicked(object sender, EventArgs e)
        {
            (BindingContext as JobOfferViewModel).GetCurrentLocation();
        }

        private void SendButton_Clicked(object sender, EventArgs e)
        {
            // send sms code
            //try
            //{
            //    await Sms.ComposeAsync(new SmsMessage(Editorr.Text, Phone.Text));
            //}
            //catch
            //{
            //    await DisplayAlert("Error", "Could not send the message, please try again later", "OK");
            //}
           
        }
    }
}