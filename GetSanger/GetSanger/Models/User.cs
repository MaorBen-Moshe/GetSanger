using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public class User
    {
        public string UserID { get; set; }
        public Uri ProfilePictureUri { get; set; }
        public AppMode LastUserMode { get; set; } // if null open mode page else open client/sanger shell
        public List<Category> Categories { get; set; }
        public Location UserLocation { get; set; }
        public IList<Activity> Activities { get; set; } // sanger and user activities each mode shows its own activities
        public IList<JobOffer> JobOffers { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        public List<Rating> Ratings { get; set; }
        public Dictionary<string, bool> ActivatedMap { get; set; } // map usage ==> when sanger activate map the key is the activity id and the value is true/false (true when activated)
    }
}
