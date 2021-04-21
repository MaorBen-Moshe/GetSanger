using GetSanger.ViewModels;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as MapViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}