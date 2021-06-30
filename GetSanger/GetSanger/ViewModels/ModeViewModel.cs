using GetSanger.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class ModeViewModel : BaseViewModel
    {
        #region Commands
        public ICommand UserCommand { get; private set; }

        public ICommand SangerCommand { get; private set; }

        #endregion

        #region Constructor
        public ModeViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            CrashlyticsService crashlyticsService = (CrashlyticsService) AppManager.Instance.Services.GetService(typeof(CrashlyticsService));
            crashlyticsService.LogPageEntrance(nameof(ModeViewModel));
        }

        private void setCommands()
        {
            UserCommand = new Command(userCommandHelper);
            SangerCommand = new Command(sangerCommandHelper);
        }

        private async void userCommandHelper()
        {
            try
            {
                bool verified = await RunTaskWhileLoading(r_LoginServices.LoginUser(AppMode.Client));
                if (!verified)
                {
                    await r_PageService.DisplayAlert("Note", "Please verify your email to continue!", "OK");
                }
                else
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch(Exception e)
            {
                handleException(e);
            }
        }

        private async void sangerCommandHelper()
        {
            try
            {
                bool verified = await RunTaskWhileLoading(r_LoginServices.LoginUser(AppMode.Sanger));
                if (!verified)
                {
                    await r_PageService.DisplayAlert("Note", "Please verify your email to continue!", "OK");
                }
                else
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch(Exception e)
            {
                handleException(e);
            }
        }


        private async void handleException(Exception e)
        {
            string message = e.Message;
            if (e.InnerException != null)
            {
                message = string.Format("{0} \n {1}", message, e.InnerException.Message);
            }

            await r_PageService.DisplayAlert("Error", message, "OK");
        }
        #endregion
    }
}
