using GetSanger.Interfaces;
using GetSanger.Views;
using System;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class PopupService : Service
    {
        private IPopupService m_PopupService;

        public PopupService()
        {
            SetDependencies();
        }

        public void ShowPopup(ContentPage i_Page = null)
        {
            if(i_Page != null && !m_PopupService.IsLoadingPageShowing)
            {
                m_PopupService.InitPopupgPage(i_Page);
            }
            else if (!m_PopupService.IsLoadingPageShowing)
            {
                m_PopupService.ShowPopupgPage();
            }
        }

        public void HidePopup(Type i_PageType = null)
        {
            i_PageType ??= typeof(LoadingPage);
            if (m_PopupService.CurrentShownPage.GetType().Equals(i_PageType))
            {
                m_PopupService.HidePopupPage();
            }
        }

        public override void SetDependencies()
        {
            m_PopupService = DependencyService.Get<IPopupService>();
        }
    }
}
