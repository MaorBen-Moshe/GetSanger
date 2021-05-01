using GetSanger.Interfaces;
using GetSanger.Services;
using GetSanger.Views;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System;
using GetSanger.Models;
using System.Collections.ObjectModel;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(NewJobOffer), "jobOffer")]
    [QueryProperty(nameof(IsCreate), "isCreate")]
    [QueryProperty(nameof(JobCategory), "category")]
    public class JobOfferViewModel : BaseViewModel
    {
        #region Fields
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

        public JobOffer NewJobOffer { get; set; } // if IsCreate == true than the job offer is sent from the previews page

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
            get => NewJobOffer.Category;
            set => NewJobOffer.Category = value;
        }

        public bool IsCreate { get; set; } // create job offer or view exist job offer

        #endregion

        #region Constructor
        public JobOfferViewModel()
        {
            CurrentLocation = new Command(getCurrentLocation);
            JobLocation = new Command(getJobLocation);
            SendJobCommand = new Command(sendJob);
            if(IsCreate == true)
            {
                NewJobOffer = new JobOffer
                {
                    Date = DateTime.Now
                };
            }
            else
            {
                JobCategory = NewJobOffer.Category;
            }
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            if(IsCreate == false)
            {
                JobCategory = NewJobOffer.Category;
            }

            IntialCurrentLocation();
        }

        public void Disappearing()
        {
            NewJobOffer = new JobOffer
            {
                Date = DateTime.Now
            };
        }

        public async void IntialCurrentLocation()
        {
            if (IsCreate == false)
            {
                MyPlaceMark = await LocationServices.PickedLocation(NewJobOffer.Location);
                JobPlaceMark = await LocationServices.PickedLocation(NewJobOffer.JobLocation);
                return;
            }

            Location location = await LocationServices.GetCurrentLocation();
            MyPlaceMark = await LocationServices.PickedLocation(location);
        }

        public void SetLocation(Placemark i_PlaceMark)
        {
            _ = m_IsMyLocation == true ? MyPlaceMark = i_PlaceMark : JobPlaceMark = i_PlaceMark;
        }

        private async void getCurrentLocation()
        {
            m_IsMyLocation = true;
            bool answer = await r_PageService.DisplayAlert("Note", $"Are you sure {MyLocation} is not your location?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync($"/map?connectedpage={this}");
            }
        }

        private async void getJobLocation()
        {
            m_IsMyLocation = false;
            await Shell.Current.GoToAsync($"/map?connectedpage={this}");
        }

        private async void sendJob()
        {
            // check validation of fields!
            NewJobOffer.Location = MyPlaceMark.Location;
            NewJobOffer.JobLocation = MyPlaceMark.Location;
            NewJobOffer.Category = JobCategory;
            // create job offer title
            AppManager.Instance.ConnectedUser.AppendCollections(AppManager.Instance.ConnectedUser.JobOffers, new ObservableCollection<JobOffer>(await FireStoreHelper.AddJobOffer(NewJobOffer)));
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
