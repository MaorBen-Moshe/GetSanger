using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public enum ActivityStatus { Active, Pending, Rejected, Completed };

    public class Activity
    {
        public int ClientID { get; set; }
        public int SangerID { get; set; }
        public JobOffer JobOffer { get; set; }
        public string Status { get; set; }
    }
}
