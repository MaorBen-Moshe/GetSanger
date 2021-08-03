using GetSanger.Services;

namespace GetSanger.Models
{
    public enum eCategory { All, Delivery, Errands, House_Devices, Studies, Computers_And_Smartphones, Cleaning, Handiman, Electrician, Gardening, Pets, Vehicle, Beauty }; // all categories are here

    public class CategoryCell : PropertySetter
    {
        #region Fields
        private eCategory m_category;
        private bool m_Checked;
        #endregion

        public eCategory Category
        {
            get => m_category;
            set 
            { 
                SetStructProperty(ref m_category, value); 
                setCategoriesImage(); 
            }
        }

        // for sign up categories page
        public bool Checked
        {
            get => m_Checked;
            set => SetStructProperty(ref m_Checked, value);
        }

        //for categories main page (embedded image not Uri)
        public string ImageUri { get; set; }

        private void setCategoriesImage()
        {
            ImageUri = Category switch
            {
                eCategory.Errands => "IconArrangement.png",
                eCategory.Beauty => "IconBeauty.png",
                eCategory.Cleaning => "IconCleaning.png",
                eCategory.Computers_And_Smartphones => "IconComputersAndSmartphones.png",
                eCategory.Delivery => "IconDelivery.png",
                eCategory.Electrician => "IconElectrician.png",
                eCategory.Gardening => "IconGardening.png",
                eCategory.Handiman => "IconHandiman.png",
                eCategory.House_Devices => "IconHouseDevices.png",
                eCategory.Pets => "IconPets.png",
                eCategory.Studies => "IconStudies.png",
                eCategory.Vehicle => "IconVehicles.png",
                eCategory.All => "",
                _ => "",
            };
        }
    }
}