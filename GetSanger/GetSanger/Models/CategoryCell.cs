using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetSanger.Models
{
    public enum Category { All, Delivery, Arrangement, House_Devices, Studies, Computers_And_Smartphones, Cleaning, Handiman, Electrician, Gardening, Pets, Vehicle, Beauty, Other }; // all categories are here

    public class CategoryCell
    {

        public Category Category { get; set; }

        // for sign up categories page
        public bool Checked { get; set; }

        //for categories main page
        public string ImageUri { get; set; }

        public CategoryCell()
        {
            setCategoriesImage();
        }

        private void setCategoriesImage()
        {
            // set by category the image
        }
    }
}
