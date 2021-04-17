using System;
using Xamarin.Essentials;

namespace GetSanger.Models
{ 
    public class JobOffer
    {
        private static int m_counter = 0;
        private string m_JobId;

        public string JobId
        {
            get
            {
                return m_JobId;
            }

            private set
            {
                m_JobId = value;
            }
        }
        public string ClientID { get; set; }
        public Location Location { get; set; }
        public Location JobLocation { get; set; }
        public Category Category { get; set; }
        public ContactPhone ClientPhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public float JobTimeEstimation { get; set; }

        public JobOffer()
        {
            JobId = m_counter.ToString();
            m_counter++;
        }

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
