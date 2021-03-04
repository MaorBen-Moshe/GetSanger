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
                    await Launcher.OpenAsync(new Uri($"https://wa.me/{"972" + Phone.Text}?text={Editorr.Text}"));
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
    }
}