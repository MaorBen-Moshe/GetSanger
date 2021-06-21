﻿using GetSanger.Services;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System;
using GetSanger.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GetSanger.Constants;
using GetSanger.Converters;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(IsCreate), "isCreate")]
    [QueryProperty(nameof(JobCategoryString), "category")]
    [QueryProperty(nameof(JobOfferJson), "jobOffer")]
    public class JobOfferViewModel : BaseViewModel
    {
        #region Fields

        private JobOffer m_NewJobOffer;
        private Placemark m_MyPlacemark;
        private Placemark m_JobPlacemark;
        private string m_MyLocation;
        private string m_JobLocation;
        private bool m_IsMyLocation = true;
        private bool m_IsCreate;
        private string m_ProfileText;
        #endregion

        #region Commands

        public ICommand CurrentLocation { get; private set; }
        public ICommand JobLocation { get; private set; }
        public ICommand SendJobCommand { get; private set; }
        public ICommand ProfileCommand { get; private set; }

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

        public bool IsCreate // create job offer or view exist job offer
        {
            get => m_IsCreate;
            set => SetStructProperty(ref m_IsCreate, value);
        }

        public string ProfileText
        {
            get => m_ProfileText;
            set => SetClassProperty(ref m_ProfileText, value);
        }

        private User m_ConnectedUser { get; set; }

        #endregion

        #region Constructor

        public JobOfferViewModel()
        {
            setCommands();
            NewJobOffer = new JobOffer
            {
                Date = DateTime.Now,
                Title = "New Job Offer"
            };
        }

        #endregion

        #region Methods

        public override async void Appearing()
        {
            await InitialCurrentLocation();
            MessagingCenter.Subscribe<MapViewModel, Placemark>(this,Constants.Constants.LocationMessage,  (sender, args) =>
            {
                setLocation(args);
            });

            if (!IsCreate)
            {
                m_ConnectedUser = await FireStoreHelper.GetUser(NewJobOffer.ClientID);
                ProfileText = string.Format(@"{0}'s profile", m_ConnectedUser.PersonalDetails.NickName);
            }
        }

        public void Disappearing()
        {
        }

        public async Task InitialCurrentLocation()
        {
            try
            {
                if (IsCreate == false)
                {
                    MyPlaceMark = await r_LocationServices.PickedLocation(NewJobOffer.Location);
                    JobPlaceMark = await r_LocationServices.PickedLocation(NewJobOffer.JobLocation);
                    return;
                }

                Location location = await r_LocationServices.GetCurrentLocation();
                MyPlaceMark ??= await r_LocationServices.PickedLocation(location);
            }
            catch (PermissionException)
            {
                await r_PageService.DisplayAlert("Error", "Please allow location!", "OK");
            }
        }

        private void setCommands()
        {
            CurrentLocation = new Command(getCurrentLocation);
            JobLocation = new Command(getJobLocation);
            SendJobCommand = new Command(sendJob);
            ProfileCommand = new Command(moveProfile);
        }

        private void setLocation(Placemark i_PlaceMark)
        {
            _ = m_IsMyLocation == true ? MyPlaceMark = i_PlaceMark : JobPlaceMark = i_PlaceMark;
        }

        private async void moveProfile(object i_Param)
        {
            if(m_ConnectedUser != null)
            {
                await r_NavigationService.NavigateTo($"{ShellRoutes.Profile}?userid={m_ConnectedUser.UserId}");
            }
            else
            {
                await r_PageService.DisplayAlert("Error", "User is not available. please contact us!", "THANKS");
            }
        }

        private async void getCurrentLocation()
        {
            m_IsMyLocation = true;
            bool answer = await r_PageService.DisplayAlert("Note", $"Are you sure {MyLocation} is not your location?", "Yes", "No");
            if (answer)
            {
                await r_NavigationService.NavigateTo($"{ShellRoutes.Map}?isSearch={true}&isTrip={false}");
            }
        }

        private async void getJobLocation()
        {
            m_IsMyLocation = false;
            await r_NavigationService.NavigateTo($"{ShellRoutes.Map}?isSearch={true}&isTrip={false}");
        }

        private async void sendJob()
        {
            // check validation of fields!
            NewJobOffer.ClientID ??= AppManager.Instance.ConnectedUser.UserId;
            NewJobOffer.Location = MyPlaceMark.Location;
            NewJobOffer.JobLocation = MyPlaceMark.Location;
            NewJobOffer.CategoryName = NewJobOffer.Category.ToString();
            NewJobOffer.ClientPhoneNumber = AppManager.Instance.ConnectedUser.PersonalDetails.Phone;
            User.AppendCollections(AppManager.Instance.ConnectedUser.JobOffers,
                new ObservableCollection<JobOffer>(await RunTaskWhileLoading(FireStoreHelper.AddJobOffer(NewJobOffer))));
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