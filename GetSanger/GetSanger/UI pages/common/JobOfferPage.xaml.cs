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
            InitializeComponent();

            BindingContext = new JobOfferViewModel();
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