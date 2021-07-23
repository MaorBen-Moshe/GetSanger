using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public ICommand FocusMyLocationCommand { get; private set; }
        public ICommand ExitCommand { get; set; }

        #endregion

        #region Constructor
        public MapViewModel()
        {
            SetCommands();
        }
        #endregion

        #region Methods

        public override async void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(MapViewModel));
            await createMapSpan();
            if (IsSearch)
            {
                await sr_PageService.DisplayAlert("הודעה", "תלחץ על המקום הרצוי במפה", "OK");
            }

            if (IsTrip) // we already checked if sanger gave permission to client to see location
            {
                handleTripHelper();
                sr_TripHelper.StartTripThread(handleTrip, 20000);
            }
        }

        public override void Disappearing()
        {
            if (IsTrip)
            {
                sr_TripHelper.LeaveTripThread(handleTrip);
            }

            if (IsSearch)
            {
                cancelation();
            }
        }

        protected override void SetCommands()
        {
            SearchCommand = new Command(searchCom);
            MapClicked = new Command(mapClickedHelper);
            PinClicked = new Command(pinClickedHelper);
            CallTripCommand = new Command(callTripHelper);
            FocusMyLocationCommand = new Command(async (i_Object) => {
                await focusLocation(i_Object);
            });

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
                sr_DialService.PhoneNumber = sanger.PersonalDetails.Phone; // check for null value
                sr_DialService.Call();
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
                        position = positionList.FirstOrDefault();
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
            sr_LocationService.Cancelation();
        }

        // handler to handle the client asking for location from sanger
        private void handleTrip(object sender, System.Timers.ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(handleTripHelper);
        }

        private async void handleTripHelper()
        {
            try
            {
                User sanger = await FireStoreHelper.GetUser(SangerTripId);
                Position position = new Position(sanger.UserLocation.Latitude, sanger.UserLocation.Longitude);

                Pins = new ObservableCollection<Pin>
                {
                     Pins[0],
                     new Pin
                     {
                         Type = PinType.Generic,
                         Position = position,
                         Icon = BitmapDescriptorFactory.DefaultMarker(Color.SeaGreen),
                         Label = "Sanger Location",
                     }
                };

                // need to change for Destination location
                // when sanger is near to us we want to stop asking for location, 0.3 kilometers
                Location location = await sr_LocationService.GetCurrentLocation();
                if (location != null)
                {
                    if (Location.CalculateDistance(location, sanger.UserLocation, DistanceUnits.Kilometers) <= 0.2)
                    {
                        await sr_PageService.DisplayAlert("Note", "The sanger is near to your place", "Thanks");
                    }

                    if (Location.CalculateDistance(location, sanger.UserLocation, DistanceUnits.Kilometers) <= 0.05)
                    {
                        await sr_PageService.DisplayAlert("Note", "The sanger has arrived, enjoy your ingredients!", "Thanks");
                        sr_TripHelper.LeaveTripThread(handleTrip);
                        MessagingCenter.Send(this, Constants.Constants.EndActivity, sanger);
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
                Placemark placemark = await sr_LocationService.GetPickedLocation(new Location(i_Position.Latitude, i_Position.Longitude));
                string location = $"Did you choose the right place?\n {string.Format("{0}, {1} {2}", placemark.Locality, placemark.Thoroughfare, placemark.SubThoroughfare)}";
                await sr_PageService.DisplayAlert("Location Chosen",
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

        private async Task focusLocation(object i_param)
        {
            sr_LoadingService.ShowLoadingPage();
            Location location = await sr_LocationService.GetCurrentLocation();
            if (location == null)
            {
                return;
            }

            Position position = new Position(location.Latitude, location.Longitude);
            Span = new MapSpan(position, 0.01, 0.01);
            sr_LoadingService.HideLoadingPage();
        }

        private async Task createMapSpan()
        {
            try
            {
                await focusLocation(null);
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