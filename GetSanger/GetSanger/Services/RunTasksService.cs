using GetSanger.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class RunTasksService : Service, IRunTasks
    {
        private ILoadingDisplay m_PopupService;

        public async Task RunTaskWhileLoading(Task i_InnerTask, ContentPage i_OptionalLoading = null)
        {
            SetDependencies();
            try
            {
                m_PopupService.ShowLoadingPage(i_OptionalLoading);
                await i_InnerTask;
                m_PopupService.HideLoadingPage();
            }
            catch (Exception ex)
            {
                m_PopupService.HideLoadingPage();
                throw ex;
            }
        }

        public async Task<T> RunTaskWhileLoading<T>(Task<T> i_InnerTask, ContentPage i_OptionalLoading = null)
        {
            SetDependencies();
            try
            {
                m_PopupService.ShowLoadingPage(i_OptionalLoading);
                T result = await i_InnerTask;
                m_PopupService.HideLoadingPage();
                return result;
            }
            catch (Exception ex)
            {
                m_PopupService.HideLoadingPage();
                throw ex;
            }
        }

        public override void SetDependencies()
        {
            m_PopupService ??= AppManager.Instance.Services.GetService(typeof(LoadingService)) as LoadingService;
        }
    }
}