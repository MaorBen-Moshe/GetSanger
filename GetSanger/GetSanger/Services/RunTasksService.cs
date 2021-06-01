using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class RunTasksService : Service
    {
        private PopupService m_PopupService;

        public async Task RunTaskWhileLoading(Task i_InnerTask, ContentPage i_OptionalLoading = null)
        {
            if(m_PopupService == null)
            {
                SetDependencies();
            }

            try
            {
                m_PopupService.ShowPopup(i_OptionalLoading);
                await i_InnerTask;
                m_PopupService.HidePopup(i_OptionalLoading?.GetType());
            }
            catch (Exception ex)
            {
                m_PopupService.HidePopup();
                throw ex;
            }
        }

        public async Task<T> RunTaskWhileLoading<T>(Task<T> i_InnerTask, ContentPage i_OptionalLoading = null)
        {
            if (m_PopupService == null)
            {
                SetDependencies();
            }

            try
            {
                m_PopupService.ShowPopup(i_OptionalLoading);
                T result = await i_InnerTask;
                m_PopupService.HidePopup(i_OptionalLoading?.GetType());
                return result;
            }
            catch (Exception ex)
            {
                m_PopupService.HidePopup();
                throw ex;
            }
        }
        public override void SetDependencies()
        {
            m_PopupService = AppManager.Instance.Services.GetService(typeof(PopupService)) as PopupService;
        }
    }
}
