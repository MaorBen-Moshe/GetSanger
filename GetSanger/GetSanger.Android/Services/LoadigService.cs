using GetSanger.Droid.Services;
using GetSanger.Interfaces;
using GetSanger.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoadingService))]
namespace GetSanger.Droid.Services
{
    public class LoadingService : DialogService, ILoadingService
    {
        bool IsLoading = false;

        public void InitLoadingPage(ContentPage i_LoadingIndicatorPage = null) // param just for ios implementation
        {
            InitDialogPage(i_LoadingIndicatorPage ?? new LoadingPage());
            IsLoading = false;
        }

        public void ShowLoadingPage()
        {
            if(!_isInitialized && IsLoading == false)
            {
                InitLoadingPage(new LoadingPage()); // set the default loading page
                _dialog.Show();
                IsLoading = true;
            }
            else if(IsLoading == false)
            {
                _dialog.Show();
                IsLoading = true;
            }
            //else means loading page is already shown and we don't need to do anything
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