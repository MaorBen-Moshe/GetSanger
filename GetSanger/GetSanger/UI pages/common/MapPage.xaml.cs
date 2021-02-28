using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;


namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public Xamarin.Forms.Maps.Map CurrMap { get; set; }

        public MapPage(BaseViewModel i_RefPage)
        {
            BindingContext = new MapViewModel(i_RefPage);
            createMap();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await DisplayAlert("Note", "Please click on the right place", "OK", FlowDirection.MatchParent);
        }

        protected override void OnDisappearing()
        {
            (BindingContext as MapViewModel).Cancelation();
            base.OnDisappearing();
        }

        private async void createMap()
        {
            CurrMap = await (BindingContext as MapViewModel).CreateMapAsync();
            CurrMap.MapClicked += CurrMap_MapClicked;
            Content = CurrMap;
        }

        private void CurrMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            (BindingContext as MapViewModel).MapClicked(e.Position);
        }
    }
}