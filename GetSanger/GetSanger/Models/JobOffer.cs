using System;
using Xamarin.Essentials;

namespace GetSanger.Models
{ 
    public class JobOffer
    {
        public string JobId { get; set; }
        public string ClientID { get; set; }
        public Location Location { get; set; }
        public Location JobLocation { get; set; }
        public Category Category { get; set; }
        public ContactPhone ClientPhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public float JobTimeEstimation { get; set; }

        public override bool Equals(object obj)
        {
            if(!(obj is JobOffer))
            {
                throw new ArgumentException("Must provide a valid JobDetails object");
            }

            JobOffer other = obj as JobOffer;
            return JobId == other.JobId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
