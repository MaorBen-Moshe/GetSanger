using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetSanger.Models
{
    public enum Category { All, Delivery, Arrangement, House_Devices, Studies, Computers_And_Smartphones, Cleaning, Handiman, Electrician, Gardening, Pets, Vehicle, Beauty }; // all categories are here

    public class CategoryCell : PropertySetter
    {
        #region Fields
        private Category m_category;
        private bool m_Checked;
        #endregion
        public Category Category
        {
            get => m_category;
            set => SetStructProperty(ref m_category, value);
        }

        // for sign up categories page
        public bool Checked
        {
            get => m_Checked;
            set => SetStructProperty(ref m_Checked, value);
        }

        //for categories main page (embedded image not uri)
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
