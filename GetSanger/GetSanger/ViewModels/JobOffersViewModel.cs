using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
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
        private string m_Notes;
        #endregion

        #region Properties
        public JobOffer CurrentConfirmedJobOffer { get; set; }
        public string Notes
        {
            get => m_Notes;
            set => SetClassProperty(ref m_Notes, value);
        }
        
        #endregion

        #region Commands
        public ICommand ConfirmJobOfferCommand { get; set; }
        public ICommand SelectedJobOfferCommand { get; set; }
        public ICommand DeleteMyJobOfferCommand { get; set; }
        public ICommand SendNotesCommand { get; set; }
        #endregion

        #region Constructor
        public JobOffersViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(JobOffersViewModel));
            SelectedCategoryFilterIndex = 0;
            SelectedTimeFilterIndex = 0;
            setJobOffers();
        }

        public override void Disappearing()
        {
        }

        protected override void setCommands()
        {
            base.setCommands();
            ConfirmJobOfferCommand = new Command(confirmJobOffer);
            SelectedJobOfferCommand = new Command(selectedJobOffer);
            DeleteMyJobOfferCommand = new Command(deleteMyJobOfferCommand);
            SendNotesCommand = new Command(sendNotes);
            FilterSelectedCommand = new Command(filterSelected);
        }

        private async void sendNotes()
        {
            try
            {
                CurrentConfirmedJobOffer.SangerNotes = Notes;
                Activity activity = new Activity
                {
                    JobDetails = CurrentConfirmedJobOffer,
                    SangerID = AuthHelper.GetLoggedInUserId(),
                    SangerName = AppManager.Instance.ConnectedUser.PersonalDetails.NickName,
                    ClientID = CurrentConfirmedJobOffer.ClientID,
                    Status = eActivityStatus.Pending,
                    LocationActivatedBySanger = false
                };

                AppManager.Instance.ConnectedUser.Activities.Append<ObservableCollection<Activity>, Activity>(new ObservableCollection<Activity>(await RunTaskWhileLoading(FireStoreHelper.AddActivity(activity))));
                await r_PageService.DisplayAlert("Note", "Your request has been sent!", "Thanks");
                setJobOffers();
                CurrentConfirmedJobOffer = null;
                await PopupNavigation.Instance.PopAsync();
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:sendNotes", "Error", e.Message);
            }
        }

        protected async override void filterSelected(object i_Param)
        {
            try
            {
                eCategory category = (eCategory)Enum.Parse(typeof(eCategory), CategoriesFilterList[SelectedCategoryFilterIndex]);
                if (category.Equals(eCategory.All))
                {
                    FilteredCollection = new ObservableCollection<JobOffer>(AllCollection);
                }
                else
                {
                    FilteredCollection = new ObservableCollection<JobOffer>(
                        from job in AllCollection
                        where job.Category.Equals(category)
                        select job
                        );
                }

                if (TimeFilterList[SelectedTimeFilterIndex].Equals(k_Newest))
                {
                    FilteredCollection = new ObservableCollection<JobOffer>(FilteredCollection.OrderByDescending(job => job.Date));
                }
                else
                {
                    FilteredCollection = new ObservableCollection<JobOffer>(FilteredCollection.OrderBy(job => job.Date));
                }
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(JobOffersViewModel)}:filterSelected", "Error", e.Message);
            }
        }

        private async void confirmJobOffer(object i_Param)
        {
            try
            {
                CurrentConfirmedJobOffer = i_Param as JobOffer;
                if (AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger))
                {
                    await PopupNavigation.Instance.PushAsync(new SangerNotesView(this));
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
                if (AppManager.Instance.CurrentMode.Equals(eAppMode.Client))
                {
                    await r_PageService.DisplayAlert("Warning",
                                                     "Are you sure?",
                                                     "Yes",
                                                     "No",
                                                     async (answer) =>
                                                     {
                                                         if (answer)
                                                         {
                                                             JobOffer job = i_Param as JobOffer;
                                                             AppManager.Instance.ConnectedUser.JobOffers.Remove(job);
                                                             AllCollection.Remove(job);
                                                             IsVisibleViewList = AllCollection.Count > 0;
                                                             await RunTaskWhileLoading(FireStoreHelper.DeleteJobOffer(job.JobId));
                                                         }
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
                    await r_NavigationService.NavigateTo(ShellRoutes.ViewJobOffer + $"?jobOffer={json}");
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