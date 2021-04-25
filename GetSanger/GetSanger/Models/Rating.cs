using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models
{
    public class Rating : PropertySetter
    {
        #region Fields
        private int m_Score;
        private string m_RatingOwnerId;
        private string m_RatingWriterId;
        private string m_Description;
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

        public override bool Equals(object obj)
        {
            if (!(obj is Rating))
            {
                throw new ArgumentException("Argument is not of type rating");
            }

            Rating other = obj as Rating;
            return RatingOwnerId.Equals(other.RatingOwnerId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}