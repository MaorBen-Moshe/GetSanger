using GetSanger.Droid.Services;
using GetSanger.Interfaces;
using GetSanger.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(PopupService))]
namespace GetSanger.Droid.Services
{
    public class PopupService : DialogService, IPopupService
    {
        bool IsLoading = false;

        public void InitPopupgPage(ContentPage i_PopupPage = null) // param just for ios implementation
        {
            InitDialogPage(i_PopupPage ?? new LoadingPage());
            IsLoading = false;
        }

        public void ShowPopupgPage()
        {
            if(!_isInitialized && IsLoading == false)
            {
                InitPopupgPage(new LoadingPage()); // set the default loading page
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

        public void HidePopupPage()
        {
            if (_isInitialized && IsLoading == true)
            {
                _dialog.Hide();
                IsLoading = false;
            }
        }
    }
}