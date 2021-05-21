using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorReportPage : ContentPage
    {
        public EditorReportPage(ProfileViewModel i_Binding)
        {
            InitializeComponent();

            BindingContext = i_Binding;
        }
    }
}