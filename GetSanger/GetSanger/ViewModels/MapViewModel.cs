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
        private readonly IPageService r_PageService;
        private readonly BindableMap r_Map;

        private BaseViewModel ConnecetedPage { get; set; }

        public LocationService LocationServices { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public MapViewModel(BaseViewModel i_RefPage, ref BindableMap i_Map)
        {
            LocationServices = new LocationService();
            r_PageService = new PageServices();
            ConnecetedPage = i_RefPage;
            SearchCommand = new Command(SearchCom);
            r_Map = i_Map;
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
                    r_Map.PinsSource.Clear();
                    r_Map.PinsSource.Add(new Pin
                    {
                        Type = PinType.Place,
                        Position = position,
                        Label = $"Chosen Place" 
                    });
                    r_Map.PinsSource[0].MarkerClicked += (object sender, PinClickedEventArgs e) => locationPicked((sender as Pin).Position); 
                    r_Map.MapSpan = new MapSpan(position, 0.01, 0.01);
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
    }
}
