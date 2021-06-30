using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
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
            setJobOffers();
        }

        private void setCommands()
        {
            ConfirmJobOfferCommand = new Command(confirmJobOffer);
            SelectedJobOfferCommand = new Command(selectedJobOffer);
            DeleteMyJobOfferCommand = new Command(deleteMyJobOfferCommand);
            SendNotesCommand = new Command(sendNotes);
        }

        private async void sendNotes()
        {
            CurrentConfirmedJobOffer.SangerNotes = Notes;
            Activity activity = new Activity
            {
                JobDetails = CurrentConfirmedJobOffer,
                SangerID = AuthHelper.GetLoggedInUserId(),
                ClientID = CurrentConfirmedJobOffer.ClientID,
                Status = ActivityStatus.Pending,
                LocationActivatedBySanger = false
            };

            AppManager.Instance.ConnectedUser.Activities.Append<ObservableCollection<Activity>, Activity>(new ObservableCollection<Activity>(await RunTaskWhileLoading(FireStoreHelper.AddActivity(activity))));
            await r_PageService.DisplayAlert("Note", "Your request has been sent!", "Thanks");
            await PopupNavigation.Instance.PopAsync();
            setJobOffers();
            CurrentConfirmedJobOffer = null;
        }

        private async void confirmJobOffer(object i_Param)
        {
            CurrentConfirmedJobOffer = i_Param as JobOffer;
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Sanger))
            {
                await PopupNavigation.Instance.PushAsync(new SangerNotesView(this));
            }
        }

        private async void deleteMyJobOfferCommand(object i_Param)
        {
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Client))
            {
                bool answer = await r_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No");
                if (answer)
                {
                    JobOffer job = i_Param as JobOffer;
                    AppManager.Instance.ConnectedUser.JobOffers.Remove(job);
                    Collection.Remove(job);
                    IsVisibleViewList = Collection.Count > 0;
                    await RunTaskWhileLoading(FireStoreHelper.DeleteJobOffer(job.JobId)); 
                }
            }
        }

        protected override void refreshList()
        {
            setJobOffers();
            IsListRefreshing = false;
        }

        private async void selectedJobOffer(object i_Param)
        {
            JobOffer current = i_Param as JobOffer;
            string json = ObjectJsonSerializer.SerializeForPage(current);
            await r_NavigationService.NavigateTo(ShellRoutes.ViewJobOffer + $"?jobOffer={json}");
        }

        private async void setJobOffers()
        {
            ObservableCollection<JobOffer> currentList = null;
            switch (AppManager.Instance.CurrentMode)
            {
                case AppMode.Client:
                    currentList = new ObservableCollection<JobOffer>(await RunTaskWhileLoading(FireStoreHelper.GetUserJobOffers(AuthHelper.GetLoggedInUserId())));
                    break;
                case AppMode.Sanger:
                    List<JobOffer> temp = await RunTaskWhileLoading(getSangerJobOffers());
                    currentList = new ObservableCollection<JobOffer>(temp);
                    break;
            }

            Collection = new ObservableCollection<JobOffer>(currentList.OrderByDescending(joboffer => joboffer.Date));
            SearchCollection = new ObservableCollection<JobOffer>(Collection);
            IsVisibleViewList = Collection.Count > 0;
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
