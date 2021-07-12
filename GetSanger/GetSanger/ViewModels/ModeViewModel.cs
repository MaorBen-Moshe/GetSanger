using GetSanger.Extensions;
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
            r_CrashlyticsService.LogPageEntrance(nameof(ModeViewModel));
        }

        public void SetBackBehavior()
        {
            if (AuthHelper.IsLoggedIn())
            {
                AuthHelper.SignOut();
            }
        }

        protected override void setCommands()
        {
            UserCommand = new Command(userCommandHelper);
            SangerCommand = new Command(sangerCommandHelper);
        }

        private async void userCommandHelper()
        {
            try
            {
                bool verified = await RunTaskWhileLoading(r_LoginServices.LoginUser(eAppMode.Client));
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
                await e.LogAndDisplayError($"{nameof(ModeViewModel)}:userCommandHelper", "Error", e.Message);
            }
        }

        private async void sangerCommandHelper()
        {
            try
            {
                bool verified = await RunTaskWhileLoading(r_LoginServices.LoginUser(eAppMode.Sanger));
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
                await e.LogAndDisplayError($"{nameof(ModeViewModel)}:sangerCommandHelper", "Error", e.Message);
            }
        }
        #endregion
    }
}