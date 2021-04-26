using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Net;

namespace GetSanger.ViewModels
{
    

    [QueryProperty(nameof(UserId), "userid")]
    public class ProfileViewModel : BaseViewModel
    {
        #region Fields
        private User m_CurrenUser;
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
            //Test();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            //setUser();
            Test();
        }
        public void Test()
        {
            CurrentUser = new User();
            CurrentUser.PersonalDetails = new PersonalDetails
            {
                NickName = "Refael",
                Gender = GenderType.Male,
                Phone = new ContactPhone("0526460006"),
                Birthday = new DateTime(1993, 09, 13)
            };

            UserImage = ImageSource.FromFile("drawable/defaultAvatar.png");
            
           // UserImage = ImageSource.FromUri(CurrentUser.ProfilePictureUri) ?? ImageSource.FromResource("Drawable/defaultAvatar.png") ;
            Rating rating = new Rating
            {
                Score = 4,
                RatingOwnerId = "311219372",
                Description = "Refael Will Be good"

            };
            Rating rating2 = new Rating
            {
                Score = 5,
                RatingOwnerId = "308431725",
                Description = "Maor is a Hoder"

            };
            CurrentUser.Ratings.Add(rating);
            CurrentUser.Ratings.Add(rating2);

            //AverageRating = 4;


           // UserImage = CurrentUser.ProfilePictureUri;
            //NickName = CurrentUser.PersonalDetails.NickName;
            //Gender = CurrentUser.PersonalDetails.Gender;
            //PhoneNumber = CurrentUser.PersonalDetails.Phone;
            //Birthday = CurrentUser.PersonalDetails.Birthday;
            //Ratings = new ObservableCollection<Rating>(CurrentUser.Ratings);

            AverageRating = 3; // we will use getAverage 

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
            if (String.IsNullOrEmpty(UserId))
            {
                throw new ArgumentException("User details aren't available.");
            }


            CurrentUser = await FireStoreHelper.GetUser(UserId);
            if(CurrentUser == null)
            {
                throw new ArgumentException("User details aren't available.");
            }

            UserImage = ImageSource.FromUri(CurrentUser.ProfilePictureUri);
            if(UserImage == null) // if there isn't profile picture - we set an defalt avatar
            {
                
            }
            Placemark placemark = await LocationServices.PickedLocation(CurrentUser.UserLocation);
            UserLocation = $"{placemark.Locality}, {placemark.CountryName}";
            AverageRating = getAverage();
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
                Report report = new Report
                {
                    ReporterId = AppManager.Instance.ConnectedUser.UserID,
                    ReportedId = CurrentUser.UserID, 
                    Reason = option
                };
                await FireStoreHelper.AddReport(report);
                sendMail(report.Reason);
                await r_PageService.DisplayAlert("Note", "Your Report has been sent.", "Thanks");
            }
        }

        private void sendMail(ReportOption i_Reason)
        {
            SmtpClient smtp = new SmtpClient
            {
                Host = "Mail",
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(Constants.Constants.GetSangerMail, Constants.Constants.GetSangerMailPassword)
            };

            string body = $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} with id: {AppManager.Instance.ConnectedUser.UserID} report on: \n" +
                             $"{CurrentUser.PersonalDetails.NickName} with id: {CurrentUser.UserID}, about the reason: {i_Reason}.";

            var mailMessage = new MailMessage
            {
                Body = body,
                From = new MailAddress(Constants.Constants.GetSangerMail),
                Subject = $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} Report Message",
                Priority = MailPriority.Normal
            };

            mailMessage.To.Add(AppManager.Instance.ConnectedUser.Email);
            smtp.SendAsync(mailMessage, null);
        }

        private async void addRating(object i_Param)
        {
            await Shell.Current.GoToAsync($"{ShellRoutes.AddRating}?ratedUser={CurrentUser}");
        }

        private async void refreshList()
        {
            CurrentUser.Ratings = new ObservableCollection<Rating>(await FireStoreHelper.GetRatings(CurrentUser.UserID));
            IsListRefreshing = false;
        }

        #endregion
    }
}
