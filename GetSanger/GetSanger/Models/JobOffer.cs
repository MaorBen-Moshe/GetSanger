using System;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public class JobOffer
    {
        public int ClientID { get; set; }
        public Location Location { get; set; }
        public Location JobLocation { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public ContactPhone ClientPhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public float JobTimeEstimation { get; set; }
    }
}
