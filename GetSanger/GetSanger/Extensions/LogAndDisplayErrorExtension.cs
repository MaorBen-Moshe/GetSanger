using System;
using GetSanger.Services;
using System.Text;
using System.Threading.Tasks;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using GetSanger.Interfaces;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace GetSanger.Extensions
{
    public static class LogAndDisplayErrorExtension
    {
        private const string SeparationLine = "-------------------------------------------------------------------------------------------";
        private static readonly ICrashlyticsDisplay sr_CrashesService;

        static LogAndDisplayErrorExtension()
        {
            sr_CrashesService = new CrashlyticsService();
        }


        public static async Task LogAndDisplayError(this Exception i_Exception, string i_NameOfClassCrashes = null, string i_Header = null,
            string i_CustomMessage = null, bool i_IsAcceptDisplay = true)
        {
            sr_CrashesService.SetUserId(AppManager.Instance.ConnectedUser != null ? AppManager.Instance.ConnectedUser.UserId : "0");
            sr_CrashesService.AddCustomLogMessage($"Crashed in class: {i_NameOfClassCrashes}");
            sr_CrashesService.RecordException(i_Exception);
            sr_CrashesService.SetCustomKey("ClassName", i_NameOfClassCrashes);
            sr_CrashesService.SetCustomKey("eAppMode", AppManager.Instance.CurrentMode.ToString());
            IPageService service = AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
            if (i_IsAcceptDisplay)
            {
                await service.DisplayAlert(i_Header, i_CustomMessage, "OK");
            }
            else
            {
                await service.DisplayAlert(i_Header, i_CustomMessage);
            }

            ILoadingDisplay loading = AppManager.Instance.Services.GetService(typeof(LoadingService)) as LoadingService;
            loading.HideLoadingPage();
        }

        private static string GetErrorLogString(Exception i_Exception)
        {
            StringBuilder loggedExceptionStringBuilder = new StringBuilder();
            Exception innerExceptionFinder = i_Exception;
            Exception toBeInnermostException = i_Exception;
            int exceptionCounter = 0;

            while (innerExceptionFinder != null)
            {
                for (int i = 0; i <= exceptionCounter; i++)
                {
                    loggedExceptionStringBuilder
                        .Append("-");
                }

                loggedExceptionStringBuilder
                    .Append(innerExceptionFinder.GetType())
                    .Append(": ")
                    .Append(innerExceptionFinder.Message)
                    .Append("\n");

                if (innerExceptionFinder.InnerException != null)
                {
                    toBeInnermostException = innerExceptionFinder;
                }

                innerExceptionFinder = innerExceptionFinder.InnerException;
                exceptionCounter++;
            }

            loggedExceptionStringBuilder
                .Append("[StackTrace:]")
                .Append(toBeInnermostException.StackTrace)
                .Append("\n")
                .Append(SeparationLine)
                .Append("\n\n");

            return loggedExceptionStringBuilder.ToString();
        }
    }
}