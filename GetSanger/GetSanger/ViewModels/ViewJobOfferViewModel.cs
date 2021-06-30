using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(JobJson), "jobOffer")]
    public class ViewJobOfferViewModel : BaseViewModel
    {
        #region Fields

        private JobOffer m_JobOffer;
        private string m_ProfileText;
        private string m_MyLocation;
        private string m_JobLocation;
        private bool m_IsMyJobOffer;

        #endregion

        #region Properties

        public JobOffer Job
        {
            get => m_JobOffer;
            set => SetClassProperty(ref m_JobOffer, value);
        }

        public string JobJson
        {
            set
            {
                if (value != null)
                {
                    Job = ObjectJsonSerializer.DeserializeForPage<JobOffer>(value);
                }
            }
        }

        public bool IsMyjobOffer
        {
            get => m_IsMyJobOffer;
            set => SetStructProperty(ref m_IsMyJobOffer, value);
        }

        public string ProfileText
        {
            get => m_ProfileText;
            set => SetClassProperty(ref m_ProfileText, value);
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

        #endregion

        #region Commands

        public ICommand ProfileCommand { get; private set; }

        #endregion

        #region Constructor

        public ViewJobOfferViewModel()
        {
            setCommands();
        }

        #endregion

        #region Methods

        public override async void Appearing()
        {
            CrashlyticsService crashlyticsService = (CrashlyticsService) AppManager.Instance.Services.GetService(typeof(CrashlyticsService));
            crashlyticsService.LogPageEntrance(nameof(ViewJobOfferViewModel));

            await initData();
        }

        public void Disappearing()
        {
        }

        private void setCommands()
        {
            ProfileCommand = new Command(moveProfile);
        }

        private async Task initData()
        {
            //r_LoadingService.ShowPopup();
            ProfileText ??= string.Format(@"{0}'s profile", Job.ClientName);

            if (Job.Location != null)
            {
                Placemark myPlace = await r_LocationServices.PickedLocation(Job.Location);
                MyLocation ??= string.Format("{0}, {1} {2}", myPlace.Locality, myPlace.Thoroughfare, myPlace.SubThoroughfare);
            }

            if (Job.JobLocation != null)
            {
                Placemark jobPlacemark = await r_LocationServices.PickedLocation(Job.JobLocation);
                WorkLocation ??= string.Format("{0}, {1} {2}", jobPlacemark.Locality, jobPlacemark.Thoroughfare, jobPlacemark.SubThoroughfare);
            }

            IsMyjobOffer = AppManager.Instance.ConnectedUser.UserId == Job.ClientID;
            //r_LoadingService.HidePopup();
        }

        private async void moveProfile(object i_Param)
        {
            if (Job.ClientID != null)
            {
                await r_NavigationService.NavigateTo($"{ShellRoutes.Profile}?userid={Job.ClientID}");
            }
            else
            {
                await r_PageService.DisplayAlert("Error", "User is not available. please contact us!", "THANKS");
            }
        }

        #endregion
    }
}