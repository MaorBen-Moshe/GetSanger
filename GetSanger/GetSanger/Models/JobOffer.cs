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
    }
}
