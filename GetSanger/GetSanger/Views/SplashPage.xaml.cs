using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        private bool m_ConnectivtyChanged = false;

        public SplashPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BaseViewModel).Appearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as SplashViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}