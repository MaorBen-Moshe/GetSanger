using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
                if(value != null)
                {
                    Job = ObjectJsonSerializer.DeserializeForPage<JobOffer>(value);
                }
            }
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

        public override void Appearing()
        {
            initData();
        }

        public void Disappearing()
        {
        }

        private void setCommands()
        {
            ProfileCommand = new Command(moveProfile);
        }

        private async void initData()
        {
            var popup = AppManager.Instance.Services.GetService(typeof(LoadingService)) as LoadingService;
            popup.ShowPopup();
            User user = await FireStoreHelper.GetUser(Job.ClientID);
            ProfileText = string.Format(@"{0}'s profile", user.PersonalDetails.NickName);
            Placemark myPlace = await r_LocationServices.PickedLocation(Job.Location);
            Placemark jobPlacemark = await r_LocationServices.PickedLocation(Job.JobLocation);
            MyLocation = string.Format("{0}, {1} {2}", myPlace.Locality, myPlace.Thoroughfare, myPlace.SubThoroughfare);
            WorkLocation = string.Format("{0}, {1} {2}", jobPlacemark.Locality, jobPlacemark.Thoroughfare, jobPlacemark.SubThoroughfare);
            popup.HidePopup();
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
