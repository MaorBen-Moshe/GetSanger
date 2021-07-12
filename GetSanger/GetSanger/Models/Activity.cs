using GetSanger.Services;
using System;

namespace GetSanger.Models
{
    public enum eActivityStatus { Pending, Active, Rejected, Completed };

    public class Activity : PropertySetter
    {
        #region Fields
        private string m_ClientId;
        private string m_SangertId;
        private eActivityStatus m_Status;
        private JobOffer m_JobDetails;
        private bool m_LocationActivatedBySanger;
        private string m_SangerName;
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
        public string SangerName // the Name of the client is in jobdetails
        {
            get => m_SangerName;
            set => SetClassProperty(ref m_SangerName, value);
        }
        public JobOffer JobDetails
        {
            get => m_JobDetails;
            set => SetClassProperty(ref m_JobDetails, value);
        }
        public eActivityStatus Status
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
            return obj is Activity activity &&
                   ActivityId == activity.ActivityId &&
                   ClientID == activity.ClientID &&
                   SangerID == activity.SangerID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ActivityId, ClientID, SangerID);
        }
    }
}
