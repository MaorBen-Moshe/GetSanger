using GetSanger.Services;
using System;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public class JobOffer : PropertySetter
    {
        #region Fields

        private string m_ClientId;
        private string m_ClientName;
        private string m_Title;
        private Location m_Location;
        private Location m_JobLocation;
        private eCategory m_Category;
        private string m_Phone;
        private DateTime m_Date;
        private string m_Description;
        private string m_SangerNotes;

        #endregion

        public string JobId { get; set; }

        public string Title
        {
            get => m_Title;
            set => SetClassProperty(ref m_Title, value);
        }

        public string ClientID
        {
            get => m_ClientId;
            set => SetClassProperty(ref m_ClientId, value);
        }

        public string ClientName
        {
            get => m_ClientName;
            set => SetClassProperty(ref m_ClientName, value);
        }

        public Location Location
        {
            get => m_Location;
            set => SetClassProperty(ref m_Location, value);
        }

        public Location JobLocation
        {
            get => m_JobLocation;
            set => SetClassProperty(ref m_JobLocation, value);
        }

        public eCategory Category
        {
            get => m_Category;
            set => SetStructProperty(ref m_Category, value);
        }

        public string ClientPhoneNumber
        {
            get => m_Phone;
            set => SetClassProperty(ref m_Phone, value);
        }

        public DateTime Date
        {
            get => m_Date;
            set => SetStructProperty(ref m_Date, value);
        }

        public string Description
        {
            get => m_Description;
            set => SetClassProperty(ref m_Description, value);
        }

        public string SangerNotes
        {
            get => m_SangerNotes;
            set => SetClassProperty(ref m_SangerNotes, value);
        }

        public string CategoryName { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is JobOffer))
            {
                return false;
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