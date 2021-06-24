using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using GetSanger.Interfaces;
using GetSanger.Views;
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
        private bool m_IsListRefreshing;
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

        public bool IsListRefreshing
        {
            get => m_IsListRefreshing;
            set => SetStructProperty(ref m_IsListRefreshing, value);
        }

        #endregion

        #region Commands
        public ICommand CallCommand { get; set; }

        public ICommand SendMessageCommand { get; set; }

        public ICommand ReportUserCommand { get; set; }

        public ICommand AddRatingCommand { get; set; }

        public ICommand RefreshingCommand { get; set; }

        #endregion

        #region Constructor
        public ProfileViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            setUser();
        }

        private void setCommands()
        {
            CallCommand = new Command(callUser);
            AddRatingCommand = new Command(addRating);
            ReportUserCommand = new Command(reportUser);
            SendMessageCommand = new Command(sendMessageToUser);
            RefreshingCommand = new Command(refreshList);
        }

        private async void setUser()
        {
            try
            {
                r_LoadingService.ShowPopup();
                if (string.IsNullOrEmpty(UserId))
                {
                    throw new ArgumentException("User details aren't available.");
                }


                CurrentUser = await FireStoreHelper.GetUser(UserId);
                if (CurrentUser == null)
                {
                    throw new ArgumentException("User details aren't available.");
                }

                UserImage = r_PhotoDisplay.DisplayPicture(CurrentUser.ProfilePictureUri);
                Placemark placemark = await r_LocationServices.PickedLocation(CurrentUser.UserLocation);
                UserLocation = $"{placemark.Locality}, {placemark.CountryName}";
                AverageRating = getAverage();
                IsListRefreshing = false;
            }
            catch(Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                r_LoadingService.HidePopup();
            }
        }

        private int getAverage()
        {
            int avg = 0;
            foreach(var rating in CurrentUser.Ratings)
            {
                avg += rating.Score;
            }

            if(CurrentUser.Ratings.Count > 0)
            {
                avg /= CurrentUser.Ratings.Count;
            }
            
            return (avg > 0) ? avg : 1;
        }

        private async void callUser(object i_Param)
        {
            if (!string.IsNullOrEmpty(CurrentUser.PersonalDetails.Phone))
            {
                r_DialService.PhoneNumber = CurrentUser.PersonalDetails.Phone;
                r_DialService.Call();
                return;
            }

            await r_PageService.DisplayAlert("Note", "User does not provide phone number!", "OK");
        }

        private async void sendMessageToUser(object i_Param)
        {
            string json = ObjectJsonSerializer.SerializeForPage(CurrentUser);
            await r_NavigationService.NavigateTo($"{ShellRoutes.ChatView}?user={json}");
        }

        private async void reportUser(object i_Param)
        {   
            string response = await r_PageService.DisplayActionSheet("Please choose the reason:", "Cancel", null, AppManager.Instance.GetListOfEnumNames(typeof(ReportOption)).ToArray());
            if(Enum.TryParse(response, out ReportOption option))
            {
                try
                {
                    m_CurrentReport = new Report
                    {
                        ReporterId = AppManager.Instance.ConnectedUser.UserId,
                        ReportedId = CurrentUser.UserId,
                        Reason = option
                    };

                    string answer = await r_PageService.DisplayPrompt("Write your details:", "please add info to contact with you", "text here..."); 
                    if(answer != null)
                    {
                        m_CurrentReport.ReportMessage = answer;
                        await RunTaskWhileLoading(FireStoreHelper.AddReport(m_CurrentReport));
                        await r_PageService.DisplayAlert("Note", "Your Report has been sent to admin.", "Thanks");
                    }
                }
                catch (Exception e)
                {
                    await r_PageService.DisplayAlert("Oh No", e.Message, "Sorry");
                }
            }
        }

        private async void addRating(object i_Param)
        {
            string json = ObjectJsonSerializer.SerializeForPage(CurrentUser);
            await r_NavigationService.NavigateTo($"{ShellRoutes.AddRating}?ratedUser={json}");
        }

        private async void refreshList()
        {
            try
            {
                CurrentUser.Ratings = new ObservableCollection<Rating>(await RunTaskWhileLoading(FireStoreHelper.GetRatings(CurrentUser.UserId)));
                IsListRefreshing = false;
            }
            catch
            {
                IsListRefreshing = false;
            }
        }

        #endregion
    }
}
