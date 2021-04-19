using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(UserId), "userid")]
    public class ProfileViewModel : BaseViewModel
    {
        #region Fields
        private User m_CurrenUser;
        private ImageSource m_UserImage;
        private string m_Location;
        //personal detailes
        private string m_NickName;
        private ContactPhone m_PhonNumber;
        private GenderType m_Geneder;
        private DateTime m_Birthday;
        private Image m_ProfileImage;
        private List<Rating> m_RatingList;

        #endregion

        #region Properties
        public string UserId { get; set; }

        public User CurrentUser
        {
            get => m_CurrenUser;
            set => SetClassProperty(ref m_CurrenUser, value);
        }

        public Image ProfileImage
        {
            get => m_ProfileImage;
            set => SetClassProperty(ref m_ProfileImage, value);
        }

        public GenderType Gender
        {
            get => m_Geneder;
            set => SetStructProperty(ref m_Geneder, value);
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
        #endregion

        #region Commands
        #endregion

        #region Constructor
        public ProfileViewModel()
        {
            //setUser();
            //test - should be deleted
            CurrentUser = new User
            {
                PersonalDetails = new PersonalDetails
                {
                    Nickname = "Refael",
                    Gender = GenderType.Male, 
                    Birthday = new DateTime(1993,9,13),
                    Phone = new ContactPhone("0526460006")
                }
            };
            m_NickName = CurrentUser.PersonalDetails.Nickname;
            m_PhonNumber = CurrentUser.PersonalDetails.Phone;
            m_Geneder = CurrentUser.PersonalDetails.Gender;
            m_Birthday = CurrentUser.PersonalDetails.Birthday;
            //m_UserImage = new Image(m_CurrenUser.ProfilePictureUri);
            m_RatingList = CurrentUser.Ratings;

        }
        #endregion

        #region Methods

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
        }
        #endregion
    }
}
