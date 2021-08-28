using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorPopup : PopupPage
    {
        public EditorPopup(string i_Description, string i_PlaceHolder)
        {
            InitializeComponent();

            Background = null;
            BindingContext = new EditorViewModel(i_PlaceHolder, i_Description, i_PlaceHolder);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as PopupBaseViewModel).Appearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as PopupBaseViewModel).Disappearing();
        }
    }
}