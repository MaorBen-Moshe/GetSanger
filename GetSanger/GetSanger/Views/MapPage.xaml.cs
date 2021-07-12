using GetSanger.ViewModels;
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
            (BindingContext as BaseViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}