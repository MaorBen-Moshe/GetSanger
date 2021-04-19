using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models
{
    public class Rating
    {
        // score between 1-5.
        public float Score { get; set; }

        public User m_RatingOwnerId { get; set; }

        public string Description { get; set; }
    }
}
