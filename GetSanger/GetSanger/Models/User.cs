using GetSanger.Models.chat;
using GetSanger.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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

        public string UserId { get; set; }
        public string RegistrationToken { get; set; }
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
        public ObservableCollection<eCategory> Categories { get; set; }
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
            Categories = new ObservableCollection<eCategory>();
        }

        public override string ToString()
        {
            return PersonalDetails.NickName;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is User))
            {
                return false;
            }

            User other = obj as User;
            return UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static void AppendCollections<T>(ObservableCollection<T> i_FirstCollection, ObservableCollection<T> i_SecondCollection)
        {
            foreach (T item in i_SecondCollection)
            {
                i_FirstCollection.Add(item);
            }
        }
    }
}
