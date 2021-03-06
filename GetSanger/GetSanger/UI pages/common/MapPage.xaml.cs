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

            BindingContext = new MapViewModel(i_RefPage);
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
    }
}