using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface ILoadingDisplay
    {
        void ShowLoadingPage(ContentPage i_Page = null);

        void HideLoadingPage();
    }
}
