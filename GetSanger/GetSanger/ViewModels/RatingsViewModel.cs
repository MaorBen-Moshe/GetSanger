using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(IsMyRatings), "isMyRatings")]
    [QueryProperty(nameof(Id), "id")]
    public class RatingsViewModel : ListBaseViewModel<Rating>
    {
        #region Fields
        private bool m_IsMyRatings;
        private string m_Id;
        #endregion

        #region Properties
        public bool IsMyRatings
        {
            get => m_IsMyRatings;
            set => SetStructProperty(ref m_IsMyRatings, value);
        }

        public string Id
        {
            get => m_Id;
            set => SetClassProperty(ref m_Id, value);
        }
        #endregion

        #region Commands
        public ICommand SelectedRatingCommand { get; set; }
        #endregion

        #region Constructor
        public RatingsViewModel()
        {
            SetCommands();
        }
        #endregion

        #region Methods

        public override async void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(RatingsViewModel));
            setRatings();
            if (IsMyRatings)
            {
                await r_PageService.DisplayAlert("Note", "Click on rating to move to the writer's profile!", "OK");
            }
        }

        public override void Disappearing()
        {
            setFilterIndices();
        }

        protected override void refreshList()
        {
            setRatings();
            IsListRefreshing = false;
        }

        protected override void SetCommands()
        {
            base.SetCommands();
            SelectedRatingCommand = new Command(selectedRating);
        }

        protected override void filterSelected(object i_Param)
        {
            filterByTIme(rating => rating.TimeAdded);
        }

        private async void selectedRating(object i_Param)
        {
            try
            {
                if (IsMyRatings && i_Param is Rating rating)
                {
                    await r_NavigationService.NavigateTo(ShellRoutes.Profile + $"?userid={rating.RatingWriterId}");
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(RatingsViewModel)}:selectedRating", "Error", e.Message);
            }
            finally
            {
                SelectedItem = null;
            }
        }

        private async void setRatings()
        {
            try
            {
                List<Rating> ratingLst = await RunTaskWhileLoading(FireStoreHelper.GetRatings(Id));
                AllCollection = new ObservableCollection<Rating>(ratingLst.OrderByDescending(rating => rating.TimeAdded));
                FilteredCollection = new ObservableCollection<Rating>(AllCollection);
                SearchCollection = new ObservableCollection<Rating>(AllCollection);
                if (IsMyRatings)
                {
                    AppManager.Instance.ConnectedUser.Ratings = new ObservableCollection<Rating>(ratingLst);
                }

                IsVisibleViewList = AllCollection.Count > 0;
                setFilterIndices();
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(RatingsViewModel)}:setRatings", "Error", e.Message);
            }
        }

        #endregion
    }
}