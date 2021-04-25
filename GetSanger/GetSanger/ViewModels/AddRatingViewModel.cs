using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(RatedUser), "ratedUser")]
    public class AddRatingViewModel : BaseViewModel
    {
        #region Properties
        public User RatedUser { get; set; }

        public Rating NewRating { get; set; }
        #endregion

        #region Commands
        public ICommand AddRatingCommand { get; set; }
        #endregion

        #region Constructor
        public AddRatingViewModel()
        {
            AddRatingCommand = new Command(addRating);
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            NewRating = new Rating
            {
                Score = 1
            };
        }

        private async void addRating(object i_Param)
        {
            NewRating.RatingWriterId = AppManager.Instance.ConnectedUser.UserID;
            NewRating.RatingOwnerId = RatedUser.UserID;

            await RunTaskWhileLoading(FireStoreHelper.AddRating(NewRating));
            await r_PageService.DisplayAlert("Note", "Rating added successfully!", "Thanks");
            await GoBack();
        }
        #endregion
    }
}
