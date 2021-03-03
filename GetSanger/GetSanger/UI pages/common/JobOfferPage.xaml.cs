using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.ViewModels;
using Xamarin.Essentials;

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

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            //send sms code
            try
            {
                bool answer = await DisplayAlert("Message From", "Please choose the app:", "Whatsapp", "Default App");
                if (answer)
                {
                    sendWhatsapp(Editorr.Text, Phone.Text);
                }
                else
                {
                    await Xamarin.Essentials.Sms.ComposeAsync(new SmsMessage(Editorr.Text, Phone.Text));
                }
            }
            catch
            {
                await DisplayAlert("Error", "Could not send the message, please try again later", "OK");
            }

        }

        private async void sendWhatsapp(string i_Message, string i_Phone)
        {
            string uri = $"https://wa.me/{"972"+i_Phone}?text={i_Message}";
            await Launcher.OpenAsync(new Uri(uri));
        }
    }
}