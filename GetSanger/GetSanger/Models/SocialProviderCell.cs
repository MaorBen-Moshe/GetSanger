using GetSanger.Services;
using Xamarin.Forms;

namespace GetSanger.Models
{
    public class SocialProviderCell : PropertySetter
    {
        private eSocialProvider m_SocialProvider;
        private ImageSource m_Image;

        public eSocialProvider SocialProvider
        {
            get => m_SocialProvider;
            set
            {
                SetStructProperty(ref m_SocialProvider, value);
                setPictureForProvider();
            }
        }

        public ImageSource Image
        {
            get => m_Image;
            set => SetClassProperty(ref m_Image, value);
        }

        private void setPictureForProvider()
        {
            string picture = "";
            switch (SocialProvider)
            {
                case eSocialProvider.Email: picture = "emailIcon.png"; break;
                case eSocialProvider.Facebook: picture = "facebookIcon.png"; break;
                case eSocialProvider.Google: picture = "googleIcon.png"; break;
                case eSocialProvider.Apple: picture = "appleIcon.png"; break;
            }

            if (!string.IsNullOrWhiteSpace(picture))
            {
                Image = ImageSource.FromFile(picture);
            }
        }
    }
}
