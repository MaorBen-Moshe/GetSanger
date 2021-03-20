using GetSanger.Droid.Services;
using GetSanger.Interfaces;
using GetSanger.UI_pages.common;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoadingService))]
namespace GetSanger.Droid.Services
{
    public class LoadingService : DialogService, ILoadingService
    {
        bool IsLoading = false;

        public void InitLoadingPage(ContentPage loadingIndicatorPage = null) // param just for ios implementation
        {
            InitDialogPage(new LoadingPage());
            IsLoading = false;
        }

        public void ShowLoadingPage()
        {
            if (_isInitialized && IsLoading == false)
            {
                _dialog.Show();
                IsLoading = true;
            }
        }

        public void HideLoadingPage()
        {
            if (_isInitialized && IsLoading == true)
            {
                _dialog.Hide();
                IsLoading = false;
            }
        }
    }
}