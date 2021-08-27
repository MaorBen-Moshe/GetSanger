using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorPopup : PopupPage
    {
        public EditorPopup(string i_Description, string i_PlaceHolder)
        {
            InitializeComponent();

            editor.Text = i_Description;
            editor.Placeholder = i_PlaceHolder ?? "details here...";
            title.Text = i_PlaceHolder ?? "Details:";
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}