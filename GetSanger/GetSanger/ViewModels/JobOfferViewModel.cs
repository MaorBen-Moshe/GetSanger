using GetSanger.Interfaces;
using GetSanger.Services;
using GetSanger.Views;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System;
using GetSanger.Models;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(JobCategory), "category")]
    public class JobOfferViewModel : BaseViewModel
    {
        #region Fields
        private Placemark m_MyPlacemark;
        private Placemark m_JobPlacemark;
        private string m_MyLocation;
        private string m_JobLocation;
        private Category m_JobCategory;
        private bool m_IsMyLocation = true;
        private DateTime m_JobDate;
        private string m_JobDescription;
        #endregion

        #region Commands

        public ICommand CurrentLocation { get; private set; }
        public ICommand JobLocation { get; private set; }
        public ICommand SendJobCommand { get; private set; }
        #endregion

        #region Properties

        public DateTime JobDate
        {
            get => m_JobDate;
            set => SetStructProperty(ref m_JobDate, value);
        }

        public string JobDescription
        {
            get => m_JobDescription;
            set => SetClassProperty(ref m_JobDescription, value);
        }

        public Placemark MyPlaceMark
        {
            get
            {
                return m_MyPlacemark;
            }

            set
            {
                m_MyPlacemark = value;
                MyLocation = placemarkValidation(m_MyPlacemark);
            }
        }

        public Placemark JobPlaceMark
        {
            get
            {
                return m_JobPlacemark;
            }

            set
            {
                m_JobPlacemark = value;
                WorkLocation = placemarkValidation(m_JobPlacemark);
            }
        }

        public string MyLocation
        {
            get { return m_MyLocation; }
            set { SetClassProperty(ref m_MyLocation, value); }
        }

        public string WorkLocation
        {
            get { return m_JobLocation; }
            set { SetClassProperty(ref m_JobLocation, value); }
        }

        public Category JobCategory
        {
            get => m_JobCategory;
            set => SetStructProperty(ref m_JobCategory, value);
        }

        #endregion

        #region Constructor
        public JobOfferViewModel()
        {
            CurrentLocation = new Command(GetCurrentLocation);
            JobLocation = new Command(GetJobLocation);
            SendJobCommand = new Command(SendJob);
        }
        #endregion

        #region Methods

        public async void IntialCurrentLocation()
        {
            Location location = await LocationServices.GetCurrentLocation();
            MyPlaceMark = await LocationServices.PickedLocation(location);
        }

        public void SetLocation(Placemark i_PlaceMark)
        {
            _ = m_IsMyLocation == true ? MyPlaceMark = i_PlaceMark : JobPlaceMark = i_PlaceMark;
        }

        public async void GetCurrentLocation()
        {
            m_IsMyLocation = true;
            bool answer = await r_PageService.DisplayAlert("Note", $"Are you sure {MyLocation} is not your location?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync($"/map?connectedpage={this}");
            }
        }

        public async void GetJobLocation()
        {
            m_IsMyLocation = false;
            await Shell.Current.GoToAsync($"/map?connectedpage={this}");
        }

        public async void SendJob()
        {
            // check all entries are fill with data
            Activity current = new Activity
            {
                JobDetails = new JobOffer
                {
                    Category = JobCategory,
                    Location = MyPlaceMark.Location,
                    JobLocation = JobPlaceMark.Location,
                    ClientID = AuthHelper.GetLoggedInUserId(),
                    ClientPhoneNumber = AppManager.Instance.ConnectedUser.PersonalDetails.Phone,
                    Date = JobDate,
                    Description = JobDescription
                },

                Status = ActivityStatus.Pending,
                ClientID = AuthHelper.GetLoggedInUserId(),
                Title = $"{JobCategory} job on {JobDate}"
            };

            AppManager.Instance.ConnectedUser.Activities.Add(current);
            await RunTaskWhileLoading(FireStoreHelper.AddJobOffer(current.JobDetails));
            await RunTaskWhileLoading(FireStoreHelper.AddActivity(current));
            r_PushService.SendTAllTopic(current.JobDetails.Category.ToString(), current);
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

        public override void Appearing()
        {
            JobDate = DateTime.Now;
            IntialCurrentLocation();
        }
        #endregion
    }
}
