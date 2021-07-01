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

            editor.Text = i_Description;
            editor.Placeholder = i_PlaceHolder ?? "details here...";
        }
    }
}