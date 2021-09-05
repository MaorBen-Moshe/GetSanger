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
        private TimeSpan m_Time;
        private string m_Description;
        private int m_MinPrice;
        private int m_MaxPrice;

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

        public Location FromLocation
        {
            get => m_Location;
            set => SetClassProperty(ref m_Location, value);
        }

        public Location DestinationLocation
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

        public TimeSpan Time
        {
            get => m_Time;
            set => SetStructProperty(ref m_Time, value);
        }

        public string Description
        {
            get => m_Description;
            set => SetClassProperty(ref m_Description, value);
        }

        public string CategoryName { get; set; }

        public int MinPrice
        {
            get => m_MinPrice;
            set => SetStructProperty(ref m_MinPrice, value);
        }

        public int MaxPrice
        {
            get => m_MaxPrice;
            set => SetStructProperty(ref m_MaxPrice, value);
        }

        public override bool Equals(object obj)
        {
            return obj is JobOffer offer &&
                   JobId == offer.JobId &&
                   ClientID == offer.ClientID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(JobId, ClientID);
        }
    }
}