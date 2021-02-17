using GetSanger.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;


namespace GetSanger.UI_pages.common
{ 
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private MapRenderer MapRend { get; set; }

        public Xamarin.Forms.Maps.Map CurrMap { get; set; }

        public MapPage()
        {
            InitializeComponent();

            MapRend = new MapRenderer();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            Location location = await MapRend.GetCurrentLocation();
            Position position = new Position(location.Latitude, location.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            
            CurrMap = new Xamarin.Forms.Maps.Map(mapSpan)
            {
                MapType = MapType.Street,
                IsShowingUser = true
            };

            CurrMap.MapClicked += CurrMap_MapClicked;

            Content = CurrMap;

            await DisplayAlert("Note", "Please click on the right place", "OK", FlowDirection.MatchParent);
        }

        private async void CurrMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            Placemark placemark = await MapRend.PickedLocation(new Location(e.Position.Latitude, e.Position.Longitude));
            string location = $"Did you choose the right place?\n {placemark}";
            bool answer = await DisplayAlert("Location Chosen", location, "Yes", "No");
            if (answer)
            {
                Application.Current.MainPage = await Navigation.PopAsync();
                if ((Application.Current.MainPage as JobOfferPage).MyPlaceMark == null)
                {
                    (Application.Current.MainPage as JobOfferPage).MyPlaceMark = placemark;
                }
                else
                {
                    (Application.Current.MainPage as JobOfferPage).JobPlaceMark = placemark;
                }
            }
        }

        protected override void OnDisappearing()
        {
            MapRend.Cancelation();
            base.OnDisappearing();
        }
    }
}