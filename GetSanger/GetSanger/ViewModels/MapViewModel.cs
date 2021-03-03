using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GetSanger.Controls;
using GetSanger.Interfaces;
using GetSanger.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GetSanger.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private IPageService m_PageService;
        private ObservableCollection<Pin> m_Pins = new ObservableCollection<Pin>();
        private BindableMap m_Map;

        private BaseViewModel ConnecetedPage { get; set; }

        public LocationService LocationServices { get; private set; }

        public ICommand SearchCommand { get; private set; }
        
        public ObservableCollection<Pin> MapPins 
        {
            get => m_Pins;
            set => SetClassProperty(ref m_Pins, value);
        }

        public MapViewModel(BaseViewModel i_RefPage, ref BindableMap i_Map)
        {
            LocationServices = new LocationService();
            m_PageService = new PageServices();
            ConnecetedPage = i_RefPage;
            SearchCommand = new Command(SearchCom);
            m_Map = i_Map;
        }

        public void MapClicked(Position i_Position)
        {
            locationPicked(i_Position);
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
                    m_Map.PinsSource.Add(new Pin
                    {
                        Type = PinType.Place,
                        Position = position,
                        Label = $"Chosen Place" 
                    });
                    m_Map.PinsSource[0].MarkerClicked += (object sender, PinClickedEventArgs e) => locationPicked((sender as Pin).Position); 
                    m_Map.MapSpan = new MapSpan(position, 0.01, 0.01);
                }
            }
        }

        private async void locationPicked(Position i_Position)
        {
            Placemark placemark = await LocationServices.PickedLocation(new Location(i_Position.Latitude, i_Position.Longitude));
            string location = $"Did you choose the right place?\n {string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare)}";
            bool answer = await m_PageService.DisplayAlert("Location Chosen", location, "Yes", "No");
            if (answer)
            {
                (ConnecetedPage as JobOfferViewModel).SetLocation(placemark);
                await m_PageService.PopAsync();
            }
        }

        public void Cancelation()
        {
            LocationServices.Cancelation();
        }
    }
}
