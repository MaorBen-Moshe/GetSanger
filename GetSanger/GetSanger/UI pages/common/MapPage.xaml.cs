using GetSanger.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using System.Threading;


namespace GetSanger.UI_pages.common
{ 
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public Xamarin.Forms.Maps.Map CurrMap { get; private set; }

        public MapViewModel MapVM { get; private set; }

        public MapPage()
        {
            InitializeComponent();

            MapVM = new MapViewModel();
            createMap();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await DisplayAlert("Note", "Please click on the right place", "OK", FlowDirection.MatchParent);
        }

        protected override void OnDisappearing()
        {
            MapVM.Cancelation();
            base.OnDisappearing();
        }

        private async void createMap()
        {
            CurrMap = await MapVM.CreateMapAsync();
            CurrMap.MapClicked += CurrMap_MapClicked;
            Content = CurrMap;
        }

        private async void CurrMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            Placemark placemark = await MapVM.GetLocation(e.Position.Latitude, e.Position.Longitude); 
            string location = $"Did you choose the right place?\n {placemark}";
            bool answer = await DisplayAlert("Location Chosen", location, "Yes", "No");
            if (answer)
            {
                //var page = await Navigation.PopAsync();
                //if ((page as JobOfferPage).MyPlaceMark == null)
                //{
                //    (page as JobOfferPage).MyPlaceMark = placemark;
                //}
                //else
                //{
                //    (page as JobOfferPage).JobPlaceMark = placemark;
                //}

                //Application.Current.MainPage = page;
            }
        }
    }
}