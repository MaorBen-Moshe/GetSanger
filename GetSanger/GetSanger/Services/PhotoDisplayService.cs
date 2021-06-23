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

        public Task<ImageSource> DisplayPicture(Uri i_Uri = null)
        {
            SetDependencies();
            var task = new Task<ImageSource>(() =>
            {
                try
                {
                    ImageSource image;
                    if (i_Uri != null)
                    {
                        byte[] imageData = null;

                        using (var wc = new System.Net.WebClient())
                        {
                            imageData = wc.DownloadData(i_Uri);
                        }

                        image = ImageSource.FromStream(() => new MemoryStream(imageData));
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
            });


            return task;
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
                await m_StorageHelper.SetUserProfileImage(i_User, destStream);
                await FireStoreHelper.UpdateUser(i_User);
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
