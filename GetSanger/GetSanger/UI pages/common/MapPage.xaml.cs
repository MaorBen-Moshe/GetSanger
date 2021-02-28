using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;


namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private SearchBar m_SearchBar;

        public Xamarin.Forms.Maps.Map CurrentMap { get; set; }

        public MapPage(BaseViewModel i_RefPage)
        {
            BindingContext = new MapViewModel(i_RefPage);
            createMap();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await DisplayAlert("Note", "Please click on the right place", "OK", FlowDirection.MatchParent);
        }

        protected override void OnDisappearing()
        {
            (BindingContext as MapViewModel).Cancelation();
            base.OnDisappearing();
        }

        private async void createMap()
        {
            CurrentMap = await (BindingContext as MapViewModel).CreateMapAsync();
            CurrentMap.MapClicked += CurrMap_MapClicked;
            Content = createContent();
        }

        private StackLayout createContent()
        {
            m_SearchBar = new SearchBar
            {
                Placeholder = "כתובת"
            };
            var button = new Button
            {
                CornerRadius = 20,
                Text = "חפש",  
                BackgroundColor = Color.Transparent

            };
            button.Clicked += Button_Clicked;
            var grid = new Grid
            {
                ColumnSpacing = 20
            };
            grid.Children.Add(button, 0, 0);
            grid.Children.Add(m_SearchBar, 1, 0);
            Grid.SetColumnSpan(m_SearchBar, 2);
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.5, GridUnitType.Star)
            });

            return new StackLayout
            {
                Children =
                {
                    grid,
                    CurrentMap
                }
            };
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await (BindingContext as MapViewModel).SetSearch(m_SearchBar.Text);
        }

        private void CurrMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            (BindingContext as MapViewModel).MapClicked(e.Position);
        }
    }
}