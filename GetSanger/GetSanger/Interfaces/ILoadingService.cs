using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface ILoadingService
    {
        void InitLoadingPage(ContentPage loadingIndicatorPage = null);

        void ShowLoadingPage();

        void HideLoadingPage();
    }
}
