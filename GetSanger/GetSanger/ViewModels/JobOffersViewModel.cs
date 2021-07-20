using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Utils;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class JobOffersViewModel : ListBaseViewModel<JobOffer>
    {
        #region Fields
        private bool m_IsSangerMode;
        #endregion

        #region Properties
        
        public bool IsSangerMode
        {
            get => m_IsSangerMode;
            set => SetStructProperty(ref m_IsSangerMode, value);
        }
        
        #endregion

        #region Commands
        public ICommand ConfirmJobOfferCommand { get; set; }
        public ICommand SelectedJobOfferCommand { get; set; }
        public ICommand DeleteMyJobOfferCommand { get; set; }
        #endregion

        #region Constructor
        public JobOffersViewModel()
        {
            SetCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(JobOffersViewModel));
            setJobOffers();
            MessagingCenter.Subscribe<SangerNotesViewModel>(this, Constants.Constants.SangerNotesSent, (sender) =>
            {
                setJobOffers();
            });
        }

        public override void Disappearing()
        {
            setFilterIndices();
            MessagingCenter.Unsubscribe<SangerNotesViewModel>(this, Constants.Constants.SangerNotesSent);
        }

        protected override void SetCommands()
        {
            base.SetCommands();
            ConfirmJobOfferCommand = new Command(confirmJobOffer);
            SelectedJobOfferCommand = new Command(selectedJobOffer);
            DeleteMyJobOfferCommand = new Command(deleteMyJobOfferCommand);
            FilterSelectedCommand = new Command(filterSelected);
        }

        protected async override void filterSelected(object i_Param)
        {
            try
            {
                eCategory category = (eCategory)Enum.Parse(typeof(eCategory), CategoriesFilterList[SelectedCategoryFilterIndex]);
                filterByCategory(job => job.Category.Equals(category));
                filterByTIme(job => job.Date);
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:filterSelected", "Error", e.Message);
            }
            finally
            {
                SearchCollection = new ObservableCollection<JobOffer>(FilteredCollection);
            }
        }

        private async void confirmJobOffer(object i_Param)
        {
            try
            {
                if(i_Param is JobOffer job)
                {
                    await JobOffersConfirmationHelper.ConfirmJobOffer(job);
                }
                
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:confirmJobOffer", "Error", e.Message);
            }
        }

        private async void deleteMyJobOfferCommand(object i_Param)
        {
            try
            {
                if (i_Param is JobOffer job)
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
                                                                 AllCollection.Remove(job);
                                                                 FilteredCollection.Remove(job);
                                                                 IsVisibleViewList = AllCollection.Count > 0;
                                                                 await FireStoreHelper.DeleteJobOffer(job.JobId);
                                                                 AppManager.Instance.ConnectedUser.JobOffers.Remove(job);
                                                                 sr_LoadingService.HideLoadingPage();
                                                             }
                                                         });
                    });
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:deleteMyJobOfferCommand", "Error", e.Message);
            }
        }

        protected override void refreshList()
        {
            setJobOffers();
            IsListRefreshing = false;
        }

        private async void selectedJobOffer(object i_Param)
        {
            try
            {
                if(i_Param is JobOffer current)
                {
                    string json = ObjectJsonSerializer.SerializeForPage(current);
                    await sr_NavigationService.NavigateTo(ShellRoutes.ViewJobOffer + $"?jobOffer={json}");
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:selectedJobOffer", "Error", e.Message);
            }
            finally
            {
                SelectedItem = null;
            }
        }

        private async void setJobOffers()
        {
            try
            {
                ObservableCollection<JobOffer> currentList = null;
                switch (AppManager.Instance.CurrentMode)
                {
                    case eAppMode.Client:
                        currentList = new ObservableCollection<JobOffer>(await RunTaskWhileLoading(FireStoreHelper.GetUserJobOffers(AuthHelper.GetLoggedInUserId())));
                        break;
                    case eAppMode.Sanger:
                        List<JobOffer> temp = await RunTaskWhileLoading(getSangerJobOffers());
                        currentList = new ObservableCollection<JobOffer>(temp);
                        break;
                }

                AllCollection = new ObservableCollection<JobOffer>(currentList.OrderByDescending(joboffer => joboffer.Date));
                FilteredCollection = new ObservableCollection<JobOffer>(AllCollection);
                SearchCollection = new ObservableCollection<JobOffer>(AllCollection);
                IsVisibleViewList = AllCollection.Count > 0;
                IsSangerMode = AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger);
                setFilterIndices();
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:setJobOffers", "Error", e.Message);
            }
        }

        private async Task<List<JobOffer>> getSangerJobOffers()
        {
            IEnumerable<eCategory> obsCollection = AppManager.Instance.ConnectedUser.Categories;
            string currentId = AppManager.Instance.ConnectedUser.UserId;
            List<JobOffer> joboffers = await FireStoreHelper.GetAllJobOffers(obsCollection.ToList());
            joboffers = joboffers.Where(current => current.ClientID != currentId).ToList();
            List<Activity> activities = await FireStoreHelper.GetActivities(currentId);
            AppManager.Instance.ConnectedUser.Activities = new ObservableCollection<Activity>(activities);
            List<JobOffer> toRetList = new List<JobOffer>();
            bool found = false;
            foreach (JobOffer job in joboffers)
            {
                found = false;
                foreach (Activity activity in activities)
                {
                    if (activity.JobDetails.Equals(job))
                    {
                        found = true;
                        break;
                    }
                }

                if(found == false)
                {
                    toRetList.Add(job);
                }
            }

            return toRetList;
        }
        #endregion
    }
}