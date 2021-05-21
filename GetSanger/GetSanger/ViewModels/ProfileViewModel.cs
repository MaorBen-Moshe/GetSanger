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
        private string m_ReportMessage;
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

        public string ReportMessage
        {
            get => m_ReportMessage;
            set => SetClassProperty(ref m_ReportMessage, value);
        }

        #endregion

        #region Commands
        public ICommand CallCommand { get; set; }

        public ICommand SendMessageCommand { get; set; }

        public ICommand ReportUserCommand { get; set; }

        public ICommand AddRatingCommand { get; set; }

        public ICommand RefreshingCommand { get; set; }

        public ICommand ReportExtraCommand { get; set; }
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
            ReportExtraCommand = new Command(addEditorReport);
        }

        private async void setUser()
        {
            if (String.IsNullOrEmpty(UserId))
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
            AverageRating = 3;
            IsListRefreshing = false;
        }

        private int getAverage()
        {
            int avg = 0;
            foreach(var rating in CurrentUser.Ratings)
            {
                avg += rating.Score;
            }

            return avg / CurrentUser.Ratings.Count;
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
            // navigate to app chat
            //await r_NavigationService.NavigateTo(ShellRoutes.ChatView + $"?userTo={CurrentUser}");

            // this code can be in the chat page instead of here
            if (!string.IsNullOrEmpty(CurrentUser.PersonalDetails.Phone))
            {
                r_DialService.PhoneNumber = CurrentUser.PersonalDetails.Phone;
                bool succeeded = await r_DialService.SendWhatsapp();
                if (!succeeded)
                {
                    r_DialService.SendDefAppMsg();
                }

                return;
            }

            await r_PageService.DisplayAlert("Note", "User does not provide phone number!", "OK");
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

                    IPopupService service = DependencyService.Get<IPopupService>();
                    service.InitPopupgPage(new EditorReportPage(this));
                    service.ShowPopupgPage();
                }
                catch (Exception e)
                {
                    await r_PageService.DisplayAlert("Oh No", e.Message, "Sorry");
                }
            }
        }

        private async void addEditorReport()
        {
            IPopupService service = DependencyService.Get<IPopupService>();
            service.HidePopupPage();
            m_CurrentReport.ReportMessage = ReportMessage;
            await RunTaskWhileLoading(FireStoreHelper.AddReport(m_CurrentReport));
            await r_PageService.DisplayAlert("Note", "Your Report has been sent to admin.", "Thanks");
        }

        private async void addRating(object i_Param)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.AddRating + $"ratedUser={CurrentUser}");
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
