using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models
{
    public enum Category { Delivery, Arrangement, Home, Studies, Experts, Miscellaneous, None }; // all categories are here

    public class CategoryCell
    {
        public Category Category { get; set; }

        public bool Checked { get; set; }
    }
}
