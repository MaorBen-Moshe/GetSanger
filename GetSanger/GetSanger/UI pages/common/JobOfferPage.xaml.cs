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
       
        public JobOfferPage()
        {
            InitializeComponent();
        }

        private async void LocationButton_Clicked(object sender, EventArgs e)
        {
            try
            {

                Location location_marks = await getCurrentLocation();
                IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(location_marks);
                Placemark placemark = placemarks.FirstOrDefault();
                if (placemark == null)
                {
                    m_LocationEntry.Text = "Location could not be found, please try manually add it";
                }

                m_LocationEntry.Text = placemark.ToString();
            }
            catch(Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        protected override void OnDisappearing()
        {
            if(Cts !=null && !Cts.IsCancellationRequested)
            {
                Cts.Cancel();
            }

            base.OnDisappearing();
        }
    }
}