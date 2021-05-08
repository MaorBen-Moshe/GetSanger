﻿using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(RatedUser), "ratedUser")]
    public class AddRatingViewModel : BaseViewModel
    {
        #region Fields
        private Rating m_NewRating;
        #endregion

        #region Properties
        public User RatedUser { get; set; }

        public Rating NewRating
        {
            get => m_NewRating;
            set => SetClassProperty(ref m_NewRating, value);
        }
        #endregion

        #region Commands
        public ICommand AddRatingCommand { get; set; }
        #endregion

        #region Constructor
        public AddRatingViewModel()
        {
            setCommands();
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

        private void setCommands()
        {
            AddRatingCommand = new Command(addRating);
        }

        private async void addRating(object i_Param)
        {
            NewRating.RatingWriterId = AppManager.Instance.ConnectedUser.UserId;
            NewRating.RatingOwnerId = RatedUser.UserId;

            AppManager.Instance.ConnectedUser.AppendCollections(AppManager.Instance.ConnectedUser.Ratings, new ObservableCollection<Rating>(await RunTaskWhileLoading(FireStoreHelper.AddRating(NewRating))));
            await r_PageService.DisplayAlert("Note", "Rating added successfully!", "Thanks");
            await GoBack();
        }
        #endregion
    }
}
