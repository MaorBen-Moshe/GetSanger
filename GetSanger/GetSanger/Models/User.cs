using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    class User
    {
        public int UserID { get; set; }
        public Uri ProfilePictureUri { get; set; }
        public List<string> Categoriess { get; set; }
        public List<Location> WorkLocations { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        public List<Rating> Ratings { get; set; }
        //public Dictionary<int, bool> ActivatedMap { get; set; } // map usage ==> when sanger activate map the key is the sanger id and the value is true\false (true when activated)
    }
}
