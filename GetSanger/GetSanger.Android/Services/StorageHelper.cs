using System;
using System.IO;
using System.Threading.Tasks;
//using Firebase.Storage;
using GetSanger.Droid.Services;
using GetSanger.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(StorageHelper))]

namespace GetSanger.Droid.Services
{
    public class StorageHelper : IStorage
    {
        public async Task DeleteProfileImage(string i_Path)
        {
            //StorageReference storageReference = FirebaseStorage.Instance.Reference;
            //StorageReference pathReference = storageReference.Child(i_Path);
            //await pathReference?.DeleteAsync();
        }

        public async Task<Uri> UploadAndGetCloudFilePath(Stream i_ToUpload, string i_PathToUpload)
        {
            try
            {
                //StorageReference storageReference = FirebaseStorage.Instance.Reference;
                //StorageReference pathReference = storageReference.Child(i_PathToUpload);
                //Android.Gms.Tasks.Task task = pathReference.PutStream(i_ToUpload);
                //while (task.IsComplete != true);
                //Android.Net.Uri auri = await pathReference.GetDownloadUrlAsync();
                //Uri uri = new Uri(auri.Path);
                return new Uri("");
            }
            catch
            {
                return null;
            }
        }
    }
}