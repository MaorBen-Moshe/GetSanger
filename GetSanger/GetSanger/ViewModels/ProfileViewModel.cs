using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net.Mail;

namespace GetSanger.ViewModels
{
    public enum ReportOption { Abuse, Harassment, Unprofessional, Ads, Other }; // need to fully implement

    [QueryProperty(nameof(UserId), "userid")]
    public class ProfileViewModel : BaseViewModel
    {
        #region Fields
        private User m_CurrenUser;
        private ImageSource m_UserImage;
        private string m_Location;
        private int m_AverageRating;
        private string m_NickName;
        private ContactPhone m_PhonNumber;
        private GenderType m_Geneder;
        private DateTime m_Birthday;
        private List<Rating> m_RatingList;

        #endregion

        #region Properties
        public string UserId { get; set; }

        public User CurrentUser
        {
            get => m_CurrenUser;
            set => SetClassProperty(ref m_CurrenUser, value);
        }

        public GenderType Gender
        {
            get => m_Geneder;
            set => SetStructProperty(ref m_Geneder, value);
        }

        public int AverageRating
        {
            get => m_AverageRating;
            set => SetStructProperty(ref m_AverageRating, value);
        }

        public  DateTime Birthday
        {
            get => m_Birthday;
            set => SetStructProperty(ref m_Birthday,value);
        }

        public ContactPhone PhoneNumber
        {
            get => m_PhonNumber;
            set => SetClassProperty(ref m_PhonNumber, value);
        }

        public string NickName
        {
            get => m_NickName;
            set => SetClassProperty(ref m_NickName, value);
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

        public List<Rating> Ratings
        {
            get => m_RatingList;
            set => SetClassProperty(ref m_RatingList, value);
        }
        #endregion

        #region Commands
        public ICommand CallCommand { get; set; }

        public ICommand SendMessageCommand { get; set; }

        public ICommand ReportUserCommand { get; set; }

        public ICommand AddRatingCommand { get; set; }
        #endregion

        #region Constructor
        public ProfileViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        private void setCommands()
        {
            CallCommand = new Command(callUser);
            AddRatingCommand = new Command(addRating);
            ReportUserCommand = new Command(reportUser);
            SendMessageCommand = new Command(sendMessageToUser); 
        }

        private async void setUser()
        {
            if (String.IsNullOrEmpty(UserId))
            {
                throw new ArgumentException("User details aren't available.");
            }

            CurrentUser = await FireStoreHelper.GetUser(UserId);
            UserImage = ImageSource.FromUri(CurrentUser.ProfilePictureUri);
            Placemark placemark = await LocationServices.PickedLocation(CurrentUser.UserLocation);
            UserLocation = $"{placemark.Locality}, {placemark.CountryName}";
            AverageRating = getAverage(CurrentUser);
        }

        private int getAverage(User i_User)
        {
            List<Rating> ratings = i_User.Ratings;
            int avg = 0;
            foreach(var rating in ratings)
            {
                avg += rating.Score;
            }

            return avg / ratings.Count;
        }

        private async void callUser(object i_Param)
        {
            if (!string.IsNullOrEmpty(CurrentUser.PersonalDetails.Phone.PhoneNumber))
            {
                r_DialService.PhoneNumber = CurrentUser.PersonalDetails.Phone.PhoneNumber;
                r_DialService.Call();
                return;
            }

            await r_PageService.DisplayAlert("Note", "User does not provide phone number!", "OK");
        }

        private async void sendMessageToUser(object i_Param)
        {
            if (!string.IsNullOrEmpty(CurrentUser.PersonalDetails.Phone.PhoneNumber))
            {
                r_DialService.PhoneNumber = CurrentUser.PersonalDetails.Phone.PhoneNumber;
                bool succeeded = await r_DialService.SendWhatsapp();
                if (!succeeded)
                {
                    r_DialService.SendDefMsg();
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
                SmtpClient smtp = new SmtpClient
                {
                    Host = "Mail",
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
                };

                string body = $"{AppManager.Instance.ConnectedUser.PersonalDetails.Nickname} with id: {AppManager.Instance.ConnectedUser.UserID} report on: \n" +
                                 $"{CurrentUser.PersonalDetails.Nickname} with id: {CurrentUser.UserID}, about the reason: {option.ToString()}.";

                var mailMessage = new MailMessage
                {
                    Body = body,
                    From = new MailAddress(AppManager.Instance.ConnectedUser.Email),
                    Subject = $"{AppManager.Instance.ConnectedUser.PersonalDetails.Nickname} Report Message",
                    Priority = MailPriority.Normal
                };

                mailMessage.To.Add(Constants.Constants.GetSangerMail);
                smtp.Send(mailMessage);
                await r_PageService.DisplayAlert("Note", "Your report has sent to us. we will contact you.", "Thanks");
            }
        }

        private async void addRating(object i_Param)
        {
            await Shell.Current.GoToAsync($"{ShellRoutes.AddRating}?ratedUser={CurrentUser}");
        }

        protected override void appearing(object i_Param)
        {
            setUser();
        }

        protected override void disappearing(object i_Param)
        {
        }
        #endregion
    }
}
