using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public enum ActivityStatus { Pending, ConfirmedBySanger, Active, Rejected, Completed };

    public class Activity
    {
        private static int m_counter = 0;
        private string m_ActivityId;

        public string ActivityId 
        {
            get
            {
                return m_ActivityId;
            }

            private set
            {
                m_ActivityId = value;
            }
        }
        public string ClientID { get; set; }
        public string SangerID { get; set; }
        public string Title { get; set; }
        public JobOffer JobOffer { get; set; }
        public ActivityStatus Status { get; set; }
        public bool LocationActivatedBySanger { get; set; }

        public Activity()
        {
            ActivityId = m_counter.ToString();
            m_counter++;
        }
    }
}
