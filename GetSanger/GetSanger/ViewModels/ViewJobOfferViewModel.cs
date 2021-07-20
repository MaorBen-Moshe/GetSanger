using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Utils;
using System;
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
        private bool m_IsDeliveryCategory;
        private bool m_IsSangerMode;
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

        public bool IsDeliveryCategory
        {
            get => m_IsDeliveryCategory;
            set => SetStructProperty(ref m_IsDeliveryCategory, value);
        }

        public bool IsSangerMode
        {
            get => m_IsSangerMode;
            set => SetStructProperty(ref m_IsSangerMode, value);
        }

        #endregion

        #region Commands

        public ICommand ProfileCommand { get; private set; }

        public ICommand ConfirmJobOfferCommand { get; private set; }

        public ICommand DeleteMyJobOfferCommand { get; private set; }

        #endregion

        #region Constructor

        public ViewJobOfferViewModel()
        {
            SetCommands();
        }

        #endregion

        #region Methods

        public override async void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(ViewJobOfferViewModel));
            await initData();
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            ProfileCommand = new Command(moveProfile);
            ConfirmJobOfferCommand = new Command(confirmJobOffer);
            DeleteMyJobOfferCommand = new Command(deleteJobOffer);
        }

        private async Task initData()
        {
            try
            {
                sr_LoadingService.ShowLoadingPage();
                IsDeliveryCategory = Job.Category.Equals(eCategory.Delivery);
                ProfileText ??= string.Format(@"{0}'s profile", Job.ClientName);

                if (Job.Location != null)
                {
                    Placemark myPlace = await sr_LocationService.GetPickedLocation(Job.Location);
                    MyLocation ??= string.Format("{0}, {1} {2}", myPlace.Locality, myPlace.Thoroughfare, myPlace.SubThoroughfare);
                }

                if (Job.JobLocation != null)
                {
                    Placemark jobPlacemark = await sr_LocationService.GetPickedLocation(Job.JobLocation);
                    WorkLocation ??= string.Format("{0}, {1} {2}", jobPlacemark.Locality, jobPlacemark.Thoroughfare, jobPlacemark.SubThoroughfare);
                }

                IsMyjobOffer = AppManager.Instance.ConnectedUser.UserId == Job.ClientID;
                IsSangerMode = AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger);
                sr_LoadingService.HideLoadingPage();
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ViewJobOfferViewModel)}:initData", "Error", e.Message);
            }
        }

        private async void confirmJobOffer(object i_Param)
        {
            try
            {
                await sr_PageService.DisplayAlert("Note", "Are you sure?", "Yes", "No", async (answer) =>
                {
                    if (answer)
                    {
                        await JobOffersConfirmationHelper.ConfirmJobOffer(Job);
                        await GoBack();
                    }
                });
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ViewJobOfferViewModel)}:confirmJobOffer", "Error", e.Message);
            }
        }

        private async void deleteJobOffer(object i_Param)
        {
            try
            {
                JobOffersConfirmationHelper.DeleteMyJobOfferCommand(action: async () =>
                {
                    await sr_PageService.DisplayAlert("Warning",
                                                     "Are you sure?",
                                                     "Yes",
                                                     "No",
                                                     async (answer) =>
                                                     {
                                                         if (answer)
                                                         {
                                                             sr_LoadingService.ShowLoadingPage();
                                                             await FireStoreHelper.DeleteJobOffer(Job.JobId);
                                                             AppManager.Instance.ConnectedUser.JobOffers.Remove(Job);
                                                             sr_LoadingService.HideLoadingPage();
                                                             await GoBack();
                                                         }
                                                     });
                });
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:deleteMyJobOfferCommand", "Error", e.Message);
            }
        }

        private async void moveProfile(object i_Param)
        {
            try
            {
                if (Job.ClientID != null)
                {
                    await sr_NavigationService.NavigateTo($"{ShellRoutes.Profile}?userid={Job.ClientID}");
                }
                else
                {
                    await sr_PageService.DisplayAlert("Error", "User is not available. please contact us!", "THANKS");
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ViewJobOfferViewModel)}:moveProfile", "Error", e.Message);
            }
        }

        #endregion
    }
}