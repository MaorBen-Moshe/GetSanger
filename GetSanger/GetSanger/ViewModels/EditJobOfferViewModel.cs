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
        private Placemark m_MyPlacemark;
        private Placemark m_JobPlacemark;
        private string m_MyLocation;
        private string m_JobLocation;
        private bool m_IsMyLocation = true;
        #endregion

        #region Commands

        public ICommand CurrentLocation { get; private set; }
        public ICommand JobLocation { get; private set; }
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

        public Placemark MyPlaceMark
        {
            get { return m_MyPlacemark; }

            set
            {
                m_MyPlacemark = value;
                MyLocation = placemarkValidation(m_MyPlacemark);
            }
        }

        public Placemark JobPlaceMark
        {
            get { return m_JobPlacemark; }

            set
            {
                m_JobPlacemark = value;
                WorkLocation = placemarkValidation(m_JobPlacemark);
            }
        }

        public string MyLocation
        {
            get => m_MyLocation; 
            set => SetClassProperty(ref m_MyLocation, value); 
        }

        public string WorkLocation
        {
            get => m_JobLocation; 
            set => SetClassProperty(ref m_JobLocation, value); 
        }

        public string JobCategoryString
        {
            set
            {
                eCategory category = CategoryToStringConverter.FromString(value);
                NewJobOffer.Category = category;
            }
        }
        #endregion

        #region Constructor

        public EditJobOfferViewModel()
        {
            SetCommands();
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

        public override async void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(EditJobOfferViewModel));
            await InitialCurrentLocation();
            MessagingCenter.Subscribe<MapViewModel, Placemark>(this,Constants.Constants.LocationMessage,  (sender, args) =>
            {
                setLocation(args);
            });
        }

        public override void Disappearing()
        {
        }

        public async Task InitialCurrentLocation()
        {
            try
            {
                Location location = await sr_LocationService.GetCurrentLocation();
                if(location != null)
                {
                    MyPlaceMark ??= await sr_LocationService.GetPickedLocation(location);
                    JobPlaceMark ??= MyPlaceMark;
                }
            }
            catch (PermissionException e)
            {
                await e.LogAndDisplayError($"{nameof(EditJobOfferViewModel)}:InitialCurrentLocation", "Error", "Please allow location!");
            }
        }

        protected override void SetCommands()
        {
            CurrentLocation = new Command(getCurrentLocation);
            JobLocation = new Command(getJobLocation);
            SendJobCommand = new Command(sendJob);
        }

        private void setLocation(Placemark i_PlaceMark)
        {
            if(i_PlaceMark == null)
            {
                throw new ArgumentNullException("i_PlaceMark is null");
            }

            _ = m_IsMyLocation == true ? MyPlaceMark = i_PlaceMark : JobPlaceMark = i_PlaceMark;
        }

        private async void getCurrentLocation()
        {
            try
            {
                m_IsMyLocation = true;
                await sr_PageService.DisplayAlert("Note",
                                                 $"Are you sure {MyLocation} is not your location?",
                                                 "Yes",
                                                 "No",
                                                  async (answer) =>
                                                  {
                                                      if (answer)
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
                                                  });
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(EditJobOfferViewModel)}:getCurrentLocation", "Error", e.Message);
            }
        }

        private async void getJobLocation()
        {
            try
            {
                m_IsMyLocation = false;
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
                await e.LogAndDisplayError($"{nameof(EditJobOfferViewModel)}:getJobLocation", "Error", e.Message);
            }
        }

        private async void sendJob()
        {
            try
            {
                if (MyPlaceMark != null &&
                    JobPlaceMark != null &&
                    NewJobOffer.Title != null &&
                    NewJobOffer.Description != null)
                {
                    NewJobOffer.ClientID ??= AppManager.Instance.ConnectedUser.UserId;
                    NewJobOffer.ClientName ??= AppManager.Instance.ConnectedUser.PersonalDetails.NickName;
                    NewJobOffer.Location = MyPlaceMark?.Location;
                    NewJobOffer.JobLocation = JobPlaceMark?.Location;
                    NewJobOffer.CategoryName = NewJobOffer.Category.ToString();
                    List<JobOffer> job = await RunTaskWhileLoading(FireStoreHelper.AddJobOffer(NewJobOffer));
                    AppManager.Instance.ConnectedUser.JobOffers.Append<ObservableCollection<JobOffer>, JobOffer>(new ObservableCollection<JobOffer>(job));
                    await sr_PageService.DisplayAlert("Success", "Job sent!", "Thanks");
                    string jobJson = ObjectJsonSerializer.SerializeForPage(job.FirstOrDefault());
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
            string toRet;
            if (i_Placemark == null)
            {
                toRet = "Location could not be found, please try manually add it";
            }

            toRet = string.Format("{0}, {1} {2}", i_Placemark.Locality, i_Placemark.Thoroughfare, i_Placemark.SubThoroughfare);
            return toRet;
        }

        #endregion
    }
}