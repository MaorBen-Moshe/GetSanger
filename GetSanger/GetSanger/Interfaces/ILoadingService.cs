using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface ILoadingService
    {
        void InitLoadingPage(ContentPage i_LoadingIndicatorPage = null);

        void ShowLoadingPage();

        void HideLoadingPage();
    }
}
