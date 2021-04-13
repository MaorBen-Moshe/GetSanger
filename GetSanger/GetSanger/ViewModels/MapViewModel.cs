using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(ConnecetedPage), "connectedpage")]
    public class MapViewModel : BaseViewModel
    {
        private ObservableCollection<Pin> m_Pins;
        private MapSpan m_Span;
        private bool m_IsSearch;
        private bool m_IsTrip;

        private BaseViewModel ConnecetedPage { get; set; }

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

        public bool IsSearch
        {
            get => m_IsSearch;
            set => SetStructProperty(ref m_IsSearch, value);
        }

        public bool IsTrip
        {
            get => m_IsTrip;
            set => SetStructProperty(ref m_IsTrip, value);
        }

        public ICommand SearchCommand { get; private set; }

        public ICommand MapClicked { get; private set; }

        public ICommand PinClicked { get; private set; }

        public ICommand EndTripCommand { get; private set; }

        public ICommand CallTripCommand { get; private set; }

        public MapViewModel()
        {
            SearchCommand = new Command(SearchCom);
            MapClicked = new Command(MapClickedHelper);
            PinClicked = new Command(PinClickedHelper);
            EndTripCommand = new Command(EndTripHelper);
            CallTripCommand = new Command(CallTripHelper);
            createMapSpan();
            IsSearch = ConnecetedPage is JobOfferViewModel;
            IsTrip = ConnecetedPage is ActivityViewModel;
            if (IsTrip)
            {
                LocationServices.HandleTripThread(handleTrip);
            }
        }

        public void MapClickedHelper(object i_Args)
        {
            if (IsSearch)
            {
                locationPicked((Position)i_Args);
            }
        }

        public void PinClickedHelper(object i_Args)
        {
            if (IsSearch)
            {
                locationPicked((Position)i_Args);
            }
        }

        public void EndTripHelper()
        {
            // kill the thread that getting location from a sanger if in user // or stop enabling location to user from sanger
            LocationServices.LeaveTripThread(handleTrip);
            throw new NotImplementedException();
        }

        public async void CallTripHelper()
        {
            try
            {
                User sanger = await FireStoreHelper.GetUser((ConnecetedPage as ActivityViewModel).ConnectedActivity.SangerID);
                r_DialService.PhoneNumber = sanger.PersonalDetails.Phone.PhoneNumber;
                r_DialService.Call();
            }
            catch(ArgumentNullException anex)
            {
                await r_PageService.DisplayAlert("Error", anex.Message, "Ok", null);
            }
            catch(FeatureNotSupportedException fnx)
            {
                await r_PageService.DisplayAlert("Error", fnx.Message, "Ok", null);
            }
            catch
            {
                await r_PageService.DisplayAlert("שגיאה", "משהו נכשל, תנסה שוב מאוחר יותר.", "Ok", "ok");
            }
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
                    Span = new MapSpan(position, 0.01, 0.01);
                }
            }
        }

        public void Cancelation()
        {
            LocationServices.Cancelation();
        }

        private async void handleTrip(object sender, System.Timers.ElapsedEventArgs e)
        {
            var activity = (ConnecetedPage as ActivityViewModel).ConnectedActivity;
            User sanger = await FireStoreHelper.GetUser(activity.SangerID);
            Position position = new Position(sanger.UserLocation.Latitude, sanger.UserLocation.Longitude);
            Span = new MapSpan(position, 0.01, 0.01);
            Pins.Clear();
            Pins = new ObservableCollection<Pin>
            {
                new Pin
                {
                    Type = PinType.Generic,
                    Position = Span.Center,
                    // add icon to pin
                }
            };


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
            Pins = new ObservableCollection<Pin>
            {
                new Pin
                {
                    Type = PinType.Generic,
                    Position = Span.Center,
                    Label = "My Location"
                }
            };
        }
    }
}