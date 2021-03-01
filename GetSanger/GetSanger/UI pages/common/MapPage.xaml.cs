using GetSanger.Controls;
using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;


namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage(BaseViewModel i_RefPage)
        {
            InitializeComponent();

            BindingContext = new MapViewModel(i_RefPage, ref CurrentMap);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            CurrentMap.MapClicked += CurrMap_MapClicked;
            await DisplayAlert("Note", "Please click on the right place", "OK", FlowDirection.MatchParent);
        }

        protected override void OnDisappearing()
        {
            (BindingContext as MapViewModel).Cancelation();
            base.OnDisappearing();
        }

        private void CurrMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            (BindingContext as MapViewModel).MapClicked(e.Position);
        }
    }
}