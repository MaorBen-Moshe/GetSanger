using GetSanger.Droid.Services;
using GetSanger.Interfaces;
using GetSanger.Views;
using System.Collections.Generic;
using Xamarin.Forms;

[assembly: Dependency(typeof(PopupService))]
namespace GetSanger.Droid.Services
{
    public class PopupService : DialogService, IPopupService
    {
        public Page CurrentShownPage
        {
            get
            {
                Page toRet = null;
                if(_contentPages.TryPeek(out PageDialog current))
                {
                    toRet = current.Page;
                }

                return toRet;
            }
        }

        public void InitPopupgPage(ContentPage i_PopupPage = null) // param just for ios implementation
        {
            InitDialogPage(i_PopupPage ?? new LoadingPage());
            _contentPages.Peek().IsLoading = false;
        }

        public void ShowPopupgPage()
        {
            if(_contentPages.Count == 0)
            {
                InitPopupgPage(new LoadingPage()); // set the default loading page
                _contentPages.Peek().CurrentDialog.Show();
                _contentPages.Peek().IsLoading = true;
            }
            else if(_contentPages.Peek().IsLoading == false)
            {
                _contentPages.Peek().CurrentDialog.Show();
                _contentPages.Peek().IsLoading = true;
            }
            //else means loading page is already shown and we don't need to do anything
        }

        public void HidePopupPage()
        {
            var current = _contentPages.Pop();
            if (current.IsLoading == true)
            {
                current.CurrentDialog.Hide();
            }
        }
    }
}