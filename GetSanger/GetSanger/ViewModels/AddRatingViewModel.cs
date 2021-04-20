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
        #region Fields
        private int m_Rating;
        private string m_Review;
        #endregion

        #region Properties
        public User RatedUser { get; set; }

        public int Rating
        {
            get => m_Rating;
            set => SetStructProperty(ref m_Rating, value);
        }

        public string Review
        {
            get => m_Review;
            set => SetClassProperty(ref m_Review, value);
        }
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
        private async void addRating(object i_Param)
        {
            Rating current = new Rating
            {
                Score = Rating,
                Description = Review,
                RatingWriterId = AppManager.Instance.ConnectedUser.UserID,
                RatingOwnerId = RatedUser.UserID
            };

            await RunTaskWhileLoading(FireStoreHelper.AddRating(current));
            await r_PageService.DisplayAlert("Note", "Rating added successfully!", "Thanks");
            await GoBack();
        }

        protected override void appearing(object i_Param)
        {
            Rating = 1;
        }

        protected override void disappearing(object i_Param)
        {
        }
        #endregion
    }
}
