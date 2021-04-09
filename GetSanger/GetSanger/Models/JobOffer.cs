using System;
using Xamarin.Essentials;

namespace GetSanger.Models
{ 
    public class JobOffer
    {
        public string ClientID { get; set; }
        public Location Location { get; set; }
        public Location JobLocation { get; set; }
        public Category Category { get; set; }
        public string SubCategory { get; set; }
        public ContactPhone ClientPhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public float JobTimeEstimation { get; set; }
    }
}
