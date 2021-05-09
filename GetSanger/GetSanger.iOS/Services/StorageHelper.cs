using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;
using Foundation;
using GetSanger.Interfaces;
using GetSanger.iOS.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(StorageHelper))]
namespace GetSanger.iOS.Services
{
    public class StorageHelper : IStorage
    {
        public Task DeleteProfileImage(string i_Path)
        {
            throw new NotImplementedException();
        }

        public async Task<Uri> UploadAndGetCloudFilePath(Stream i_ToUpload, string i_PathToUpload)
        {
            StorageReference storageReference = Firebase.Storage.Storage.DefaultInstance.GetRootReference();
            StorageReference pathReference = storageReference.GetChild(i_PathToUpload);
            NSData data = NSData.FromStream(i_ToUpload);
            return await new Task<Uri>(() => new Uri(""));
        }
    }
}