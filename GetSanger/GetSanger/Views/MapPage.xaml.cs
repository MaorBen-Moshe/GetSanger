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

            BindingContext = new MapViewModel();
        }
    }
}