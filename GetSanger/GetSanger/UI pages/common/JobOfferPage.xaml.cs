using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GetSanger.Models;

namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobOfferPage : ContentPage
    {
        private Placemark m_MyPlacemark;
        private Placemark m_JobPlacemark;

        public Placemark MyPlaceMark {
            get
            {
                return m_MyPlacemark;
            }

            set 
            {
                m_MyPlacemark = value;
                placemarkValidation(m_MyPlacemark, ref m_MyLocationEntry);
            } 
        }

        public Placemark JobPlaceMark
        {
            get
            {
                return m_JobPlacemark;
            }

            set
            {
                m_JobPlacemark = value;
                placemarkValidation(m_JobPlacemark, ref m_JobLocationEntry);
            }
        }

        public JobOfferPage()
        {
            InitializeComponent();
        }

        private async void LocationButton_Clicked(object sender, EventArgs e)
        {
            MapPage map = new MapPage();
            await Navigation.PushAsync(map);
        }

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Sms.ComposeAsync(new SmsMessage(Editorr.Text, Phone.Text));
            }
            catch
            {
                await DisplayAlert("Error", "Could not send the message, please try again later", "OK");
            }
           
        }

        private void placemarkValidation(Placemark i_Placemark, ref Entry i_Entry)
        {
            if (i_Placemark == null)
            {
                i_Entry.Text = "Location could not be found, please try manually add it";
            }

            i_Entry.Text = MyPlaceMark.ToString();
        }
    }
}