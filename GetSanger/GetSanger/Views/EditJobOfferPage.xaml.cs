using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.ViewModels;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditJobOfferPage : ContentPage
    {
        public EditJobOfferPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BaseViewModel).Appearing();
            if(!(BindingContext as EditJobOfferViewModel).IsDeliveryCategory)
            {
                mainStack.Children.Remove(fromStack);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as BaseViewModel).Disappearing();
        }
    }
}