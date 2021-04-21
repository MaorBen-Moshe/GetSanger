using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public class User
    {
        public string UserID { get; set; }
        [JsonIgnore]
        public string Email { get; set; }
        public Uri ProfilePictureUri { get; set; }
        public AppMode? LastUserMode { get; set; } // if null open mode page else open client/sanger shell
        public List<Category> Categories { get; set; }
        public bool IsGenericNotifications { get; set; }
        public Location UserLocation { get; set; }
        [JsonIgnore]
        public IList<Activity> Activities { get; set; } // sanger and user activities each mode shows its own activities
        [JsonIgnore]
        public IList<JobOffer> JobOffers { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        [JsonIgnore]
        public List<Rating> Ratings { get; set; }
        public Dictionary<string, bool> ActivatedMap { get; set; } // map usage ==> when sanger activate map the key is the activity id and the value is true/false (true when activated)

        public User()
        {
            JobOffers = new List<JobOffer>();
            ActivatedMap = new Dictionary<string, bool>();
            Activities = new List<Activity>();
            Ratings = new List<Rating>();
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
