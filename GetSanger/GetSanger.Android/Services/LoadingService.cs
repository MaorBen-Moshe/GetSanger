using GetSanger.Droid.Services;
using GetSanger.Interfaces;
using GetSanger.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoadingService))]
namespace GetSanger.Droid.Services
{
    public class LoadingService : DialogService, ILoadingService
    {
        private bool _isLoading = false;

        bool ILoadingService.IsLoading => _isLoading;

        public void InitLoadingPage(ContentPage i_Page = null)
        {
            InitDialogPage(i_Page ?? new LoadingPage());
            _isLoading = false;
        }

        public void ShowLoadingPage()
        {
            if (_isInitialized && _isLoading == false)
            {
                _dialog.Show();
                _isLoading = true;
            }
            else if (_isLoading == false)
            {
                initAndShow();
            }
            else
            {
                _dialog.Dismiss();
                initAndShow();
            }
        }

        public void HideLoadingPage()
        {
            if (_isInitialized && _isLoading == true)
            {
                _dialog.Dismiss();
                _isLoading = false;
            }
        }

        private void initAndShow()
        {
            InitLoadingPage();
            _dialog.Show();
            _isLoading = true;
        }
    }
}