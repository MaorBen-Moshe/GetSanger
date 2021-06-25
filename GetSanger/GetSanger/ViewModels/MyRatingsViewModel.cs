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
    public class MyRatingsViewModel : ListBaseViewModel<Rating>
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Commands
        public ICommand SelectedRatingCommand { get; set; }
        #endregion

        #region Constructor
        public MyRatingsViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public async override void Appearing()
        {
            setRatings();
            await r_PageService.DisplayAlert("Note", "Click on rating to move to the writer's profile!", "OK");
        }

        public void Disappearing()
        {
        }

        protected override void refreshList()
        {
            setRatings();
            IsListRefreshing = false;
        }

        private void setCommands()
        {
            SelectedRatingCommand = new Command(selectedRating);
        }

        private async void selectedRating(object i_Param)
        {
            Rating current = i_Param as Rating;
            await r_NavigationService.NavigateTo(ShellRoutes.Profile + $"?userid={current.RatingWriterId}");
        }

        private async void setRatings()
        {
            List<Rating> ratingLst = await RunTaskWhileLoading(FireStoreHelper.GetRatings(AppManager.Instance.ConnectedUser.UserId));
            ratingLst.OrderBy(rating => rating.TimeAdded);
            Collection = new ObservableCollection<Rating>(ratingLst);
            SearchCollection = new ObservableCollection<Rating>(Collection);
            AppManager.Instance.ConnectedUser.Ratings = new ObservableCollection<Rating>(ratingLst);
            IsVisibleViewList = Collection.Count > 0;
        }

        #endregion
    }
}
