using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public class JobOffer
    {
        public int ClientID { get; set; }
        public Location Location { get; set; }
        public string Category { get; set; }
        public ContactPhone PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public float JobTimeEstimation { get; set; }
    }
}
