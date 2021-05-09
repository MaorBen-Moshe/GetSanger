using System;
using System.IO;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IStorage
    {
        Task<Uri> UploadAndGetCloudFilePath(Stream i_ToUpload, string i_PathToUpload);

        Task DeleteProfileImage(string i_Path);
    }
}
