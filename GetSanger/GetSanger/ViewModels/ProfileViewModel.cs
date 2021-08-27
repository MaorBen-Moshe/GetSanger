using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using GetSanger.Views.popups;
using GetSanger.Extensions;
using System.Threading.Tasks;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(UserId), "userid")]
    public class ProfileViewModel : BaseViewModel
    {
        #region Fields
        private User m_CurrenUser;
        private Report m_CurrentReport;
        private int m_AverageRating;
        private ImageSource m_UserImage;
        private string m_Location;
        #endregion

        #region Properties
        public string UserId { get; set; }

        public User CurrentUser
        {
            get => m_CurrenUser;
            set => SetClassProperty(ref m_CurrenUser, value);
        }

        public int AverageRating
        {
            get => m_AverageRating;
            set => SetStructProperty(ref m_AverageRating, value);
        }

        public ImageSource UserImage
        {
            get => m_UserImage;
            set => SetClassProperty(ref m_UserImage, value);
        }

        public string UserLocation
        {
            get => m_Location;
            set => SetClassProperty(ref m_Location, value);
        }

        #endregion

        #region Commands

        public ICommand SendMessageCommand { get; set; }

        public ICommand ReportUserCommand { get; set; }

        public ICommand AddRatingCommand { get; set; }

        public ICommand ViewRatingsCommand { get; set; }

        public ICommand AboutMeCommand { get; set; }

        #endregion

        #region Constructor
        public ProfileViewModel()
        {
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(ProfileViewModel));
            setUser();
        }

        public override void Disappearing()
        {
            CurrentUser = null;
        }

        protected override void SetCommands()
        { 
            AddRatingCommand = new Command(addRating);
            ReportUserCommand = new Command(reportUser);
            SendMessageCommand = new Command(sendMessageToUser);
            ViewRatingsCommand = new Command(viewRatings);
            AboutMeCommand = new Command(aboutMe);
        }

        private async void setUser()
        {
            try
            {
                sr_LoadingService.ShowLoadingPage();
                if (string.IsNullOrEmpty(UserId))
                {
                    throw new ArgumentException("User details aren't available.");
                }
                CurrentUser = await FireStoreHelper.GetUser(UserId);
                if (CurrentUser == null)
                {
                    throw new ArgumentException("User details aren't available.");
                }

                UserImage = sr_PhotoDisplay.DisplayPicture(CurrentUser.ProfilePictureUri);
                await Task.Run(async () =>
                {
                    Placemark placemark = await sr_LocationService.GetPickedLocation(CurrentUser.UserLocation);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserLocation = $"{placemark.Locality}, {placemark.CountryName}";
                    });
                });

                AverageRating = CurrentUser.Ratings?.Count > 0 ? (int)CurrentUser.Ratings.Average(rating => rating.Score) : 1;
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ProfileViewModel)}:setUser", "Error", e.Message);
            }
            finally
            {
                sr_LoadingService.HideLoadingPage();
            }
        }

        private async void sendMessageToUser(object i_Param)
        {
            try
            {
                if(CurrentUser.IsDeleted)
                {
                    return;
                }

                string json = ObjectJsonSerializer.SerializeForPage(CurrentUser);
                await sr_NavigationService.NavigateTo($"{ShellRoutes.ChatView}?user={json}&prev={ShellRoutes.Profile}");
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ProfileViewModel)}:sendMessageToUser", "Error", e.Message);
            }
        }

        private async void reportUser(object i_Param)
        {
            if (CurrentUser.IsDeleted)
            {
                return;
            }

            string response = await sr_PageService.DisplayActionSheet("Please choose the reason:", "Cancel", null, typeof(ReportOption).GetListOfEnumNames().ToArray());
            if(Enum.TryParse(response, out ReportOption option))
            {
                try
                {
                    m_CurrentReport = new Report
                    {
                        ReporterId = AppManager.Instance.ConnectedUser.UserId,
                        ReportedId = CurrentUser.UserId,
                        Reason = option,
                        TimeReportCreated = DateTime.Now
                    };

                    sr_PageService.DisplayPrompt("Write Report Details", 
                                                 $"please add info so we will be able to process your report about {CurrentUser.PersonalDetails.NickName}",
                                                 "info here...",
                                                 async (answer) => 
                                                 {
                                                     if (answer != null)
                                                     {
                                                         m_CurrentReport.ReportMessage = answer;
                                                         await RunTaskWhileLoading(FireStoreHelper.AddReport(m_CurrentReport));
                                                         await sr_PageService.DisplayAlert("Note", "Your Report has been sent to the admin.", "Thanks");
                                                     }
                                                     else
                                                     {
                                                         await sr_PageService.DisplayAlert("Note", "Please add details to send this report!", "OK");
                                                     }
                                                 }); 
                }
                catch (Exception e)
                {
                    await e.LogAndDisplayError($"{nameof(ProfileViewModel)}:reportUser", "Oh No", e.Message);
                }
            }
        }

        private async void addRating(object i_Param)
        {
            try
            {
                if (CurrentUser.IsDeleted)
                {
                    return;
                }

                var ratingsPopup = new AddRatingPage(CurrentUser.UserId, CurrentUser.PersonalDetails.NickName);
                (ratingsPopup.BindingContext as AddRatingViewModel).RatingAddedEvent += (RatingAdded) =>
                {
                    CurrentUser.Ratings.Add(RatingAdded);
                    AverageRating = CurrentUser.Ratings?.Count > 0 ? (int)CurrentUser.Ratings.Average(rating => rating.Score) : 1;
                };

                await PopupNavigation.Instance.PushAsync(ratingsPopup);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ProfileViewModel)}:addRating", "Error", e.Message);
            }
        }

        private async void viewRatings(object i_Param)
        {
            try
            {
                await sr_NavigationService.NavigateTo($"{ShellRoutes.Ratings}?id={CurrentUser.UserId}&isMyRatings={false}");
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ProfileViewModel)}:viewRatings", "Error", e.Message);
            }
        }

        private async void aboutMe(object i_param)
        {
            try
            {
                await PopupNavigation.Instance.PushAsync(new EditorPopup(CurrentUser.PersonalDetails.About ?? "user has not fill a description", $"About {CurrentUser.PersonalDetails.NickName}"));
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ProfileViewModel)}:aboutMe", "Error", e.Message);
            }
        }

        #endregion
    }
}