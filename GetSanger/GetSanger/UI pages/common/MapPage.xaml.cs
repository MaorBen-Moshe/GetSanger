using GetSanger.ViewModels;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();

            BindingContext = new MapViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DisplayAlert("הודעה", "תלחץ על המקום הרצוי במפה", "OK", FlowDirection.MatchParent);
        }

        protected override void OnDisappearing()
        {
            (BindingContext as MapViewModel).Cancelation();
            base.OnDisappearing();
        }
    }
}