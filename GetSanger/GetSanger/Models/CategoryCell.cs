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
            set  { SetStructProperty(ref m_category, value); setCategoriesImage(); }
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
            
        }

        private void setCategoriesImage()
        {
            ImageUri = Category switch
            {
                Category.Arrangement => "IconArrangement.png",
                Category.Beauty => "IconBeauty.png",
                Category.Cleaning => "IconCleaning.png",
                Category.Computers_And_Smartphones => "IconComputersAndSmartphones.png",
                Category.Delivery => "IconDelivery.png",
                Category.Electrician => "IconElectrician.png",
                Category.Gardening => "IconGardening.png",
                Category.Handiman => "IconHandiman.png",
                Category.House_Devices => "IconHouseDevices.png",
                Category.Pets => "IconPets.png",
                Category.Studies => "IconStudies.png",
                Category.Vehicle => "IconVehicles.png",
                Category.All => "",
                _ => "",
            };
        }
    }
}
