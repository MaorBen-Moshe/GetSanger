using GetSanger.Services;
using System;

namespace GetSanger.Models
{
    public class Rating : PropertySetter
    {
        #region Fields
        private int m_Score;
        private string m_RatingOwnerId;
        private string m_RatingWriterId;
        private string m_Description;
        private string m_RatingWriterName;
        private DateTime m_TimeAdded;
        #endregion

        public string RatingId { get; set; }

        // score between 1-5.
        public int Score 
        {
            get => m_Score;
            set => SetStructProperty(ref m_Score, value);
        }

        public string RatingOwnerId
        {
            get => m_RatingOwnerId;
            set => SetClassProperty(ref m_RatingOwnerId, value);
        }

        public string RatingWriterName
        {
            get => m_RatingWriterName;
            set => SetClassProperty(ref m_RatingWriterName, value);
        }

        public string RatingWriterId
        {
            get => m_RatingWriterId;
            set => SetClassProperty(ref m_RatingWriterId, value);
        }

        public string Description
        {
            get => m_Description;
            set => SetClassProperty(ref m_Description, value);
        }

        public DateTime TimeAdded
        {
            get => m_TimeAdded;
            set => SetStructProperty(ref m_TimeAdded, value);
        }

        public override bool Equals(object obj)
        {
            return obj is Rating rating &&
                   RatingId == rating.RatingId &&
                   RatingOwnerId == rating.RatingOwnerId &&
                   RatingWriterId == rating.RatingWriterId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RatingId, RatingOwnerId, RatingWriterId);
        }
    }
}