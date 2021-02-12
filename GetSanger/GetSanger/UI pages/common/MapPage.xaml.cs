using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;


namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public event Location_Chosen location_chosen;
        
        public event Map_Disappeared map_disappeared;

        private MapRenderer MapRend { get; set; }

        public MapPage()
        {
            InitializeComponent();

            MapRend = new MapRenderer();

            m_Map.MapType = MapType.Street;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Location location = await MapRend.GetCurrentLocation();
            Position position = new Position(location.Latitude, location.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            // bind map span to xaml map
        }

        private async void m_Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            Position position = e.Position;
            Placemark placemark = await MapRend.PickedLocation(new Location(e.Position.Latitude, e.Position.Longitude));
            // sending place mark to job offer page some how.
        }
    }
}