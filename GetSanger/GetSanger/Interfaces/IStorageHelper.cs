using GetSanger.Models;
using System.IO;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IStorageHelper
    {
        Task SetUserProfileImage(User i_User, Stream i_Stream);

        Task DeleteProfileImage(string i_UserID);
    }
}