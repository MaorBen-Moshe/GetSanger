using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models
{
    public class Rating
    {
        // score between 1-5.
        public int Score { get; set; }

        public string RatingOwnerId { get; set; }

        public string RatingWriterId { get; set; }

        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            if(!(obj is Rating))
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
