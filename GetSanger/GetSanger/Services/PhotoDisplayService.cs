using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PhotoDisplayService : Service, IPhotoDisplay
    {
        private StorageHelper m_StorageHelper;

        public ImageSource DisplayPicture(Uri i_Uri = null)
        {
            SetDependencies();
            try
            {
                ImageSource image;
                if (i_Uri != null)
                {
                    image = new UriImageSource
                    {
                        Uri = i_Uri,
                        CachingEnabled = false
                    };

                    if (image == null)
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

        public async Task TryGetPictureFromUri(string i_Uri, User i_User)
        {
            SetDependencies();
            if(i_Uri != null)
            {
                using var client = new WebClient();
                var content = client.DownloadData(i_Uri);
                using var stream = new MemoryStream(content);
                var destStream = new MemoryStream();
                await stream.CopyToAsync(destStream);
                if(i_User.ProfilePictureUri == null)
                {
                    await m_StorageHelper.SetUserProfileImage(i_User, destStream);
                    await FireStoreHelper.UpdateUser(i_User);
                }
            }
        }

        public async Task TryGetPictureFromStream(User i_User)
        {
            Stream stream = await DependencyService.Get<IPhotoPicker>().GetImageStreamAsync();
            if (stream != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                await m_StorageHelper.SetUserProfileImage(i_User, memoryStream);
            }
        }

        public override void SetDependencies()
        {
            m_StorageHelper ??= AppManager.Instance.Services.GetService(typeof(StorageHelper)) as StorageHelper;
        }
    }
}
