using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(RatedUserId), "ratedUserId")]
    [QueryProperty(nameof(UserName), "ratedUserName")]
    public class AddRatingViewModel : PopupBaseViewModel
    {
        #region Events
        public event Action<Rating> RatingAddedEvent;
        #endregion

        #region Fields
        private Rating m_NewRating;

        private string m_UserName;
        #endregion

        #region Properties
        public string RatedUserId { get; set; }

        public Rating NewRating
        {
            get => m_NewRating;
            set => SetClassProperty(ref m_NewRating, value);
        }

        public string UserName
        {
            get => m_UserName;
            set => SetClassProperty(ref m_UserName, value);
        }
        #endregion

        #region Commands
        public ICommand AddRatingCommand { get; set; }
        #endregion

        #region Constructor
        public AddRatingViewModel()
        {
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(AddRatingViewModel));
            NewRating = new Rating
            {
                Score = 1
            };
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            AddRatingCommand = new Command(addRating);
        }

        private async void addRating(object i_Param)
        {
            try
            {
                if(NewRating.Description.Length == 0)
                {
                    await sr_PageService.DisplayAlert("Note", "Please write a description!", "OK");
                }
                else
                {
                    NewRating.RatingWriterId = AppManager.Instance.ConnectedUser.UserId;
                    NewRating.RatingWriterName = AppManager.Instance.ConnectedUser.PersonalDetails.NickName;
                    NewRating.RatingOwnerId = RatedUserId;
                    NewRating.TimeAdded = DateTime.Now;
                    await RunTaskWhileLoading(FireStoreHelper.AddRating(NewRating));
                    await sr_PageService.DisplayAlert("Note", "Rating added successfully!");
                    RatingAddedEvent?.Invoke(NewRating);
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AddRatingViewModel)}:addRating", "Error", e.Message, i_IsAcceptDisplay: false);
                await PopupNavigation.Instance.PopAsync();
            }
        }

        #endregion
    }
}