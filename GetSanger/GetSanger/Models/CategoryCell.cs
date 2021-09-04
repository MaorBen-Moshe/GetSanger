using GetSanger.Services;

namespace GetSanger.Models
{
    public enum eCategory { All, Delivery, Errands, House_Devices, Studies, Computers_And_Smartphones, Cleaning, Handiman, Electrician, Gardening, Pets, Vehicle, Beauty, Miscellaneous }; // all categories are here

    public class CategoryCell : PropertySetter
    {
        #region Fields
        private eCategory m_category;
        private bool m_Checked;
        private string m_ImageUri;
        #endregion

        public string CategoryString
        {
            get
            {
                return m_category.ToString().Replace("_", " ");
            }
        }

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
        public string ImageUri
        {
            get => m_ImageUri;
            set => SetClassProperty(ref m_ImageUri, value);
        }

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
                eCategory.Miscellaneous => "IconMiscellaneous.png",
                eCategory.All => "",
                _ => "",
            };
        }
    }
}