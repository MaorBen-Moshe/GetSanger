using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface ILoadingService
    {
        void InitLoadingPage(ContentPage i_PopupPage = null);

        void ShowLoadingPage();

        void HideLoadingPage();

        bool IsLoading { get; }
    }
}