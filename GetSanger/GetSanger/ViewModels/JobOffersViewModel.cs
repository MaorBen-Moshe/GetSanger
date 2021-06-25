using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class JobOffersViewModel : ListBaseViewModel<JobOffer>
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Commands
        public ICommand ConfirmJobOfferCommand { get; set; }
        public ICommand SelectedJobOfferCommand { get; set; }
        public ICommand DeleteMyJobOfferCommand { get; set; }
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
            setJobOffers();
        }

        private void setCommands()
        {
            ConfirmJobOfferCommand = new Command(confirmJobOffer);
            SelectedJobOfferCommand = new Command(selectedJobOffer);
            DeleteMyJobOfferCommand = new Command(deleteMyJobOfferCommand);
        }

        private async void confirmJobOffer(object i_Param)
        {
            JobOffer job = i_Param as JobOffer;
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Sanger))
            {
                string json = ObjectJsonSerializer.SerializeForPage(job);
                await r_NavigationService.NavigateTo(ShellRoutes.SangerNotes + $"job={json}");
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
                    IEnumerable<eCategory> obsCollection = AppManager.Instance.ConnectedUser.Categories;
                    string currentId = AppManager.Instance.ConnectedUser.UserId;
                    List<JobOffer> temp = await RunTaskWhileLoading(FireStoreHelper.GetAllJobOffers(obsCollection.ToList()));
                    temp = temp.Where(current => current.ClientID != currentId).ToList();
                    currentList = new ObservableCollection<JobOffer>(temp);
                    break;
            }

            Collection = new ObservableCollection<JobOffer>(currentList.OrderByDescending(joboffer => joboffer.Date));
            SearchCollection = new ObservableCollection<JobOffer>(Collection);
            IsVisibleViewList = Collection.Count > 0;
        }
        #endregion
    }
}
