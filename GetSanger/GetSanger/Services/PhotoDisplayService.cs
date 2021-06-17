using GetSanger.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PhotoDisplayService : Service, IPhotoDisplay
    {
        public ImageSource DisplayPicture(Uri i_Uri = null)
        {
            try
            {
                ImageSource image;
                if (i_Uri != null)
                {
                    image = ImageSource.FromUri(i_Uri);
                    if(image == null)
                    {
                        image = ImageSource.FromFile("profile.png");
                    }
                }
                else
                {
                    image = ImageSource.FromFile("profile.png");
                }

                return image;
            }
            catch
            {
                return ImageSource.FromFile("profile.png"); ;
            }
        }

        public override void SetDependencies()
        {
        }
    }
}
