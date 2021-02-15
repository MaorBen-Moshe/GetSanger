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
        private Placemark m_Placemark;

        public Placemark PlaceMark {
            get
            {
                return m_Placemark;
            }

            set 
            {
                m_Placemark = value;
                placemarkValidation();
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

        private void placemarkValidation()
        {
            if (PlaceMark == null)
            {
                m_LocationEntry.Text = "Location could not be found, please try manually add it";
            }

            m_LocationEntry.Text = PlaceMark.ToString();
        }
    }
}