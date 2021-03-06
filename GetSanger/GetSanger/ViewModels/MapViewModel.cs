using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GetSanger.Interfaces;
using GetSanger.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GetSanger.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private readonly IPageService r_PageService;
        private ObservableCollection<Pin> m_Pins;
        private MapSpan m_Span;

        private BaseViewModel ConnecetedPage { get; set; }

        public LocationService LocationServices { get; private set; }

        public ObservableCollection<Pin> Pins
        {
            get => m_Pins;
            set => SetClassProperty(ref m_Pins, value);
        }

        public MapSpan Span
        {
            get => m_Span;
            set => SetClassProperty(ref m_Span, value);
        }

        public ICommand SearchCommand { get; private set; }

        public ICommand Clicked { get; private set; }

        public MapViewModel(BaseViewModel i_RefPage)
        {
            LocationServices = new LocationService();
            r_PageService = new PageServices();
            ConnecetedPage = i_RefPage;
            SearchCommand = new Command(SearchCom);
            Clicked = new Command(MapClicked);
            createMapSpan();
            Pins = new ObservableCollection<Pin>
            {
                new Pin
                {
                    Type = PinType.Generic,
                    Position = Span.Center,
                    Label = "My Location",
                }
            };
            Pins[0].MarkerClicked += (object sender, PinClickedEventArgs e) => locationPicked((sender as Pin).Position);
        }

        public void MapClicked(object i_Args)
        {
            Position position = (i_Args as MapClickedEventArgs).Position;
            locationPicked(position);
        }

        public async void SearchCom(object i_Search)
        {
            if(i_Search is string)
            {
                Position position;
                List<Position> positionList = new List<Position>(await (new Geocoder()).GetPositionsForAddressAsync(i_Search as string));
                if (positionList.Count != 0)
                {
                    position = positionList.FirstOrDefault<Position>();
                    Pins = new ObservableCollection<Pin>{
                        Pins[0],
                        new Pin
                        {
                        Type = PinType.Place,
                        Position = position,
                        Label = $"Chosen Place"
                        }
                    };
                    Pins[1].MarkerClicked += (object sender, PinClickedEventArgs e) => locationPicked((sender as Pin).Position); 
                    Span = new MapSpan(position, 0.01, 0.01);
                }
            }
        }

        public void Cancelation()
        {
            LocationServices.Cancelation();
        }

        private async void locationPicked(Position i_Position)
        {
            Placemark placemark = await LocationServices.PickedLocation(new Location(i_Position.Latitude, i_Position.Longitude));
            string location = $"Did you choose the right place?\n {string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare)}";
            bool answer = await r_PageService.DisplayAlert("Location Chosen", location, "Yes", "No");
            if (answer)
            {
                (ConnecetedPage as JobOfferViewModel).SetLocation(placemark);
                await r_PageService.PopAsync();
            }
        }

        private async void createMapSpan()
        {
            Location location = await LocationServices.GetCurrentLocation();
            Position position = new Position(location.Latitude, location.Longitude);
            Span = new MapSpan(position, 0.01, 0.01);
        }
    }
}
