using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(IsTrip), "isTrip")]
    [QueryProperty(nameof(IsSearch), "isSearch")]
    [QueryProperty(nameof(SangerTripId), "sangerId")]
    public class MapViewModel : BaseViewModel
    {
        #region Fields
        private ObservableCollection<Pin> m_Pins;
        private MapSpan m_Span;
        private bool m_IsSearch;
        private bool m_IsTrip;
        private string m_SangerTripId;
        #endregion

        #region Properties

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

        public string SangerTripId
        {
            get => m_SangerTripId;
            set => SetClassProperty(ref m_SangerTripId, value);
        }
        #endregion

        #region Commands

        public ICommand SearchCommand { get; private set; }

        public ICommand MapClicked { get; private set; }

        public ICommand PinClicked { get; private set; }

        public ICommand CallTripCommand { get; private set; }

        public ICommand ExitCommand { get; set; }

        #endregion

        #region Constructor
        public MapViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public override async void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(MapViewModel));
            createMapSpan();
            if (IsSearch)
            {
                await r_PageService.DisplayAlert("הודעה", "תלחץ על המקום הרצוי במפה", "OK");
            }

            if (IsTrip) // we already checked if sanger gave permission to client to see location
            {
                r_TripHelper.StartTripThread(handleTrip, 350000);
            }
        }

        public override void Disappearing()
        {
            if (IsTrip)
            {
                r_TripHelper.LeaveTripThread(handleTrip);
            }

            if (IsSearch)
            {
                cancelation();
            }
        }

        protected override void setCommands()
        {
            SearchCommand = new Command(searchCom);
            MapClicked = new Command(mapClickedHelper);
            PinClicked = new Command(pinClickedHelper);
            CallTripCommand = new Command(callTripHelper);
            ExitCommand = new Command(exit);
        }

        private void mapClickedHelper(object i_Args)
        {
            if (IsSearch)
            {
                locationPicked((Position)i_Args);
            }
        }

        private void pinClickedHelper(object i_Args)
        {
            if (IsSearch)
            {
                locationPicked((Position)i_Args);
            }
        }

        private async void callTripHelper()
        {
            try
            {
                User sanger = await RunTaskWhileLoading(FireStoreHelper.GetUser(SangerTripId));
                r_DialService.PhoneNumber = sanger.PersonalDetails.Phone; // check for null value
                r_DialService.Call();
            }
            catch(ArgumentNullException anex)
            {
                await anex.LogAndDisplayError($"{nameof(MapViewModel)}:callTripHelper", "Error", anex.Message);
            }
            catch(FeatureNotSupportedException fnx)
            {
                await fnx.LogAndDisplayError($"{nameof(MapViewModel)}:callTripHelper", "Error", fnx.Message);
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(MapViewModel)}:callTripHelper", "Error", "Something went wrong! \n Please try again later.");
            }
        }

        private async void searchCom(object i_Search)
        {
            try
            {
                if (i_Search is string)
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
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(MapViewModel)}:searchCom", "Error", e.Message);
            }
        }

        private async void exit(object i_Param)
        {
            await GoBack();
        }

        private void cancelation()
        {
            r_LocationService.Cancelation();
        }

        // handler to handle the client asking for location from sanger
        private async void handleTrip(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                User sanger = await RunTaskWhileLoading(FireStoreHelper.GetUser(SangerTripId));
                Position position = new Position(sanger.UserLocation.Latitude, sanger.UserLocation.Longitude);
                Span = new MapSpan(position, 0.01, 0.01);
                Pins.Clear();
                Pins.Add(new Pin
                {
                    Type = PinType.Generic,
                    Position = Span.Center,
                    Icon = BitmapDescriptorFactory.FromBundle("PinIcon.jpeg")
                });

                // when sanger is near to us we want to stop asking for location, 0.3 kilometers
                Location location = await r_LocationService.GetCurrentLocation();
                if (location != null)
                {
                    if (Location.CalculateDistance(location, sanger.UserLocation, DistanceUnits.Kilometers) <= 0.3)
                    {
                        await r_PageService.DisplayAlert("Note", "The sanger has arrived, enjoy your ingredients!", "Thanks");
                        r_TripHelper.LeaveTripThread(handleTrip);
                        MessagingCenter.Send(this, Constants.Constants.ActivatedLocationMessage, false);
                        await GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                await ex.LogAndDisplayError($"{nameof(MapViewModel)}:handleTrip", "Error", ex.Message);
            }
        }

        private async void locationPicked(Position i_Position)
        {
            try
            {
                Placemark placemark = await r_LocationService.GetPickedLocation(new Location(i_Position.Latitude, i_Position.Longitude));
                string location = $"Did you choose the right place?\n {string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare)}";
                await r_PageService.DisplayAlert("Location Chosen",
                                                 location,
                                                 "Yes",
                                                 "No",
                                                 async (answer) =>
                                                 {
                                                     if (answer)
                                                     {
                                                         MessagingCenter.Send(this, Constants.Constants.LocationMessage, placemark);
                                                         await GoBack();
                                                     }
                                                 });
            }
            catch (Exception ex)
            {
                await ex.LogAndDisplayError($"{nameof(MapViewModel)}:locationPicked", "Error", ex.Message);
            }
        }

        private async void createMapSpan()
        {
            try
            {
                Location location = await r_LocationService.GetCurrentLocation();
                if(location == null)
                {
                    return;
                }

                Position position = new Position(location.Latitude, location.Longitude);
                Span = new MapSpan(position, 0.01, 0.01);
                Pins = new ObservableCollection<Pin>
                {
                    new Pin
                    {
                        Type = PinType.Generic,
                        Position = Span.Center,
                        Label = "My Location",
                    }
                };
            }
            catch (PermissionException e)
            {
                await e.LogAndDisplayError($"{nameof(MapViewModel)}:createMapSpan", "Error", "Please allow location services");
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(MapViewModel)}:createMapSpan", "Error", e.Message);
            }
        }
        #endregion
    }
}