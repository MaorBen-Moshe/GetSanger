using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public class User : PropertySetter
    {
        #region Fields
        private string m_Email;
        private Uri m_ProfilePictureUre;
        private bool m_IsGenericNotifications;
        private Location m_UserLocation;
        #endregion

        public string UserID { get; set; }
        public string RegistrationToken { get; set; }
        [JsonIgnore]
        public string Email
        {
            get => m_Email;
            set => SetClassProperty(ref m_Email, value);
        }
        public Uri ProfilePictureUri
        {
            get => m_ProfilePictureUre;
            set => SetClassProperty(ref m_ProfilePictureUre, value);
        }
        public AppMode? LastUserMode { get; set; } // if null open mode page else open client/sanger shell
        public ObservableCollection<Category> Categories { get; set; }
        public bool IsGenericNotifications
        {
            get => m_IsGenericNotifications;
            set => SetStructProperty(ref m_IsGenericNotifications, value);
        }
        public Location UserLocation
        {
            get => m_UserLocation;
            set => SetClassProperty(ref m_UserLocation, value);
        }
        [JsonIgnore]
        public ObservableCollection<Activity> Activities { get; set; } // sanger and user activities each mode shows its own activities
        [JsonIgnore]
        public ObservableCollection<JobOffer> JobOffers { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        [JsonIgnore]
        public ObservableCollection<Rating> Ratings { get; set; }
        public Dictionary<string, bool> ActivatedMap { get; set; } // map usage ==> when sanger activate map the key is the activity id and the value is true/false (true when activated)

        public User()
        {
            JobOffers = new ObservableCollection<JobOffer>();
            ActivatedMap = new Dictionary<string, bool>();
            Activities = new ObservableCollection<Activity>();
            Ratings = new ObservableCollection<Rating>();
            PersonalDetails = new PersonalDetails();
            IsGenericNotifications = true; // default generic notifications 
            LastUserMode = null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is User))
            {
                throw new ArgumentException("Must provide a valid User object");
            }

            User other = obj as User;
            return UserID == other.UserID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
