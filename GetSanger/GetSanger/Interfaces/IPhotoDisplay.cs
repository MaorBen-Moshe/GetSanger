using GetSanger.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface IPhotoDisplay
    {
        ImageSource DisplayPicture(Uri i_Uri = null);

        Task TryGetPictureFromUri(string i_Uri, User i_User);

        Task TryGetPictureFromStream(User i_User);
    }
}
