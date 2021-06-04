using GetSanger.Services;

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

        //for categories main page (embedded image not Uri)
        public string ImageUri { get; set; }

        public CategoryCell()
        {
            setCategoriesImage();
        }

        private void setCategoriesImage()
        {
            ImageUri = Category switch
            {
                Category.Arrangement => "",
                Category.Beauty => "",
                Category.Cleaning => "",
                Category.Computers_And_Smartphones => "",
                Category.Delivery => "",
                Category.Electrician => "",
                Category.Gardening => "",
                Category.Handiman => "",
                Category.House_Devices => "",
                Category.Pets => "",
                Category.Studies => "",
                Category.Vehicle => "",
                Category.All => "",
                _ => "",
            };
        }
    }
}
