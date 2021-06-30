using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public enum ActivityStatus { Pending, Active, Rejected, Completed };

    public class Activity : PropertySetter
    {
        #region Fields
        private string m_ClientId;
        private string m_SangertId;
        private ActivityStatus m_Status;
        private JobOffer m_JobDetails;
        private bool m_LocationActivatedBySanger;
        #endregion

        public string ActivityId { get; set; }
        public string ClientID
        {
            get => m_ClientId;
            set => SetClassProperty(ref m_ClientId, value);
        }
        public string SangerID
        {
            get => m_SangertId;
            set => SetClassProperty(ref m_SangertId, value);
        }
        public JobOffer JobDetails
        {
            get => m_JobDetails;
            set => SetClassProperty(ref m_JobDetails, value);
        }
        public ActivityStatus Status
        {
            get => m_Status;
            set => SetStructProperty(ref m_Status, value);
        }
        public bool LocationActivatedBySanger
        {
            get => m_LocationActivatedBySanger;
            set => SetStructProperty(ref m_LocationActivatedBySanger, value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Activity))
            {
                return false;
            }

            Activity other = obj as Activity;
            return ActivityId == other.ActivityId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
