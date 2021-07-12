using GetSanger.Interfaces;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class LoadingService : Service, ILoadingDisplay
    {
        private ILoadingService m_LoadingService;

        public LoadingService()
        {
            SetDependencies();
        }

        public void ShowLoadingPage(ContentPage i_Page = null)
        {
            if(i_Page != null && !m_LoadingService.IsLoading)
            {
                m_LoadingService.InitLoadingPage(i_Page);
                m_LoadingService.ShowLoadingPage();
            }
            else if (!m_LoadingService.IsLoading) 
            {
                m_LoadingService.ShowLoadingPage();
            }
        }

        public void HideLoadingPage()
        {
            m_LoadingService.HideLoadingPage();
        }

        public override void SetDependencies()
        {
            m_LoadingService ??= DependencyService.Get<ILoadingService>();
        }
    }
}
