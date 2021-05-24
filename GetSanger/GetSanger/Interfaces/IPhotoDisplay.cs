using System;
using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface IPhotoDisplay
    {
        ImageSource DisplayPicture(Uri i_Uri = null);
    }
}
