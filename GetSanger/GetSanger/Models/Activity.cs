using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public class Activity
    {
        public int ClientID { get; set; }
        public int SangerID { get; set; }
        public JobOffer JobOffer { get; set; }
        public ContactPhone SangerPhoneNumber { get; set; }
        public string Status { get; set; }
    }
}
