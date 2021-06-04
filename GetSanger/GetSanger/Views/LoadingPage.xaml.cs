using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage(string text = "Loading...", bool internetChecking = false)
        {
            InitializeComponent();

            BindingContext = new LoadingPageViewModel(text, internetChecking);
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as LoadingPageViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}