using GetSanger.Services;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System;
using GetSanger.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GetSanger.Constants;
using GetSanger.Converters;
using System.Collections.Generic;
using System.Linq;
using GetSanger.Extensions;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(JobCategoryString), "category")]
    [QueryProperty(nameof(JobOfferJson), "jobOffer")]
    public class EditJobOfferViewModel : BaseViewModel
    {
        #region Fields

        private JobOffer m_NewJobOffer;
        private Placemark m_FromPlacemark;
        private Placemark m_DestinationPlacemark;
        private string m_FromLocationString;
        private string m_DestinationLocationString;
        private bool m_IsFromLocation = true;
        private bool m_IsDeliveryCategory;
        #endregion

        #region Commands

        public ICommand FromLocationCommand { get; private set; }
        public ICommand DestinationLocationCommand { get; private set; }
        public ICommand SendJobCommand { get; private set; }

        #endregion

        #region Properties

        // if IsCreate == true than the job offer is sent from the previews page
        public JobOffer NewJobOffer 
        {
            get => m_NewJobOffer;
            set => SetClassProperty(ref m_NewJobOffer, value);
        } 

        public string JobOfferJson
        {
            set
            {
                NewJobOffer = ObjectJsonSerializer.DeserializeForPage<JobOffer>(value);
            }
        }

        public Placemark FromPlaceMark
        {
            get { return m_FromPlacemark; }

            set
            {
                m_FromPlacemark = value;
                FromLocationString = placemarkValidation(m_FromPlacemark);
            }
        }

        public Placemark DestinationPlaceMark
        {
            get { return m_DestinationPlacemark; }

            set
            {
                m_DestinationPlacemark = value;
                DestinationLocationString = placemarkValidation(m_DestinationPlacemark);
            }
        }

        public string FromLocationString
        {
            get => m_FromLocationString; 
            set => SetClassProperty(ref m_FromLocationString, value); 
        }

        public string DestinationLocationString
        {
            get => m_DestinationLocationString; 
            set => SetClassProperty(ref m_DestinationLocationString, value); 
        }

        public string JobCategoryString
        {
            set
            {
                eCategory category = CategoryToStringConverter.FromString(value);
                NewJobOffer.Category = category;
            }
        }

        public bool IsDeliveryCategory
        {
            get => m_IsDeliveryCategory;
            set => SetStructProperty(ref m_IsDeliveryCategory, value);
        }
        #endregion

        #region Constructor

        public EditJobOfferViewModel()
        {
            NewJobOffer = new JobOffer
            {
                Date = DateTime.Now,
                Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0),
                ClientPhoneNumber = AppManager.Instance.ConnectedUser.PersonalDetails.Phone,
                Title = "New Job Offer"
            };
        }

        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(EditJobOfferViewModel));
            IsDeliveryCategory = NewJobOffer.Category.Equals(eCategory.Delivery);
            InitialCurrentLocation();
            MessagingCenter.Subscribe<MapViewModel, Placemark>(this,Constants.Constants.LocationMessage, (sender, args) =>
            {
                setLocation(args);
            });
        }

        public override void Disappearing()
        {
        }

        public async void InitialCurrentLocation()
        {
            try
            {
                Location location = await sr_LocationService.GetCurrentLocation();
                if(location != null)
                {
                    DestinationPlaceMark ??= await sr_LocationService.GetPickedLocation(location);
                    FromPlaceMark ??= DestinationPlaceMark;
                }
            }
            catch (PermissionException e)
            {
                await e.LogAndDisplayError($"{nameof(EditJobOfferViewModel)}:InitialCurrentLocation", "Error", "Please allow location!");
            }
        }

        protected override void SetCommands()
        {
            FromLocationCommand = new Command(getFromLocation);
            DestinationLocationCommand = new Command(getDestinationLocation);
            SendJobCommand = new Command(sendJob);
        }

        private void setLocation(Placemark i_PlaceMark)
        {
            if(i_PlaceMark == null)
            {
                throw new ArgumentNullException("i_PlaceMark is null");
            }

            _ = m_IsFromLocation == true ? FromPlaceMark = i_PlaceMark : DestinationPlaceMark = i_PlaceMark;
        }

        private void getFromLocation()
        {
            m_IsFromLocation = true;
            locationHelper();
        }

        private void getDestinationLocation()
        {
            m_IsFromLocation = false;
            locationHelper();
        }

        private async void locationHelper()
        {
            try
            {
                bool locationGranted = await sr_LocationService.IsLocationGrantedAndAskFor() == PermissionStatus.Granted;
                if (locationGranted)
                {
                    await sr_NavigationService.NavigateTo($"{ShellRoutes.Map}?isSearch={true}&isTrip={false}");
                }
                else
                {
                    await sr_PageService.DisplayAlert("Note", "Please allow location!", "OK");
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(EditJobOfferViewModel)}:locationHelper", "Error", e.Message);
            }
        }

        private async void sendJob()
        {
            try
            {
                if (FromPlaceMark != null &&
                    DestinationPlaceMark != null &&
                    NewJobOffer.Title != null &&
                    NewJobOffer.Description != null)
                {
                    NewJobOffer.ClientID ??= AppManager.Instance.ConnectedUser.UserId;
                    NewJobOffer.ClientName ??= AppManager.Instance.ConnectedUser.PersonalDetails.NickName;
                    NewJobOffer.FromLocation = FromPlaceMark?.Location;
                    NewJobOffer.DestinationLocation = DestinationPlaceMark?.Location;
                    if (NewJobOffer.Category.Equals(eCategory.Delivery) == false)
                    {
                        NewJobOffer.FromLocation = NewJobOffer.DestinationLocation;
                    }

                    NewJobOffer.CategoryName = NewJobOffer.Category.ToString();
                    sr_LoadingService.ShowLoadingPage();
                    List<JobOffer> job = await FireStoreHelper.AddJobOffer(NewJobOffer);
                    AppManager.Instance.ConnectedUser.JobOffers.Append<ObservableCollection<JobOffer>, JobOffer>(new ObservableCollection<JobOffer>(job));
                    await sr_PageService.DisplayAlert("Success", "Job sent!", "Thanks");
                    string jobJson = ObjectJsonSerializer.SerializeForPage(job.FirstOrDefault());
                    sr_LoadingService.HideLoadingPage();
                    await GoBack();
                    await RunTaskWhileLoading(sr_NavigationService.NavigateTo($"////{ShellRoutes.JobOffers}/{ShellRoutes.ViewJobOffer}?jobOffer={jobJson}"));
                }
                else
                {
                    await sr_PageService.DisplayAlert("Error", "Please fill all the fields of the form", "OK");
                }

            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(EditJobOfferViewModel)}:sendJob", "Error", e.Message);
            }
        }

        private string placemarkValidation(Placemark i_Placemark)
        {
            return i_Placemark == null ? "Location could not be found, please try manually add it" 
                                       : string.Format("{0}, {1} {2}", i_Placemark.Locality, i_Placemark.Thoroughfare, i_Placemark.SubThoroughfare);
        }

        #endregion
    }
}