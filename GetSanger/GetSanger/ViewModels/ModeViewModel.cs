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
            SetCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(ModeViewModel));
        }

        public override void Disappearing()
        {
        }

        public void SetBackBehavior()
        {
            if (AuthHelper.IsLoggedIn())
            {
                AuthHelper.SignOut();
            }
        }

        protected override void SetCommands()
        {
            UserCommand = new Command(userCommandHelper);
            SangerCommand = new Command(sangerCommandHelper);
        }

        private void userCommandHelper()
        {
            commandHelper(eAppMode.Client);
        }

        private void sangerCommandHelper()
        {
            commandHelper(eAppMode.Sanger);
        }

        private async void commandHelper(eAppMode i_Mode)
        {
            try
            {
                bool verified = await RunTaskWhileLoading(sr_LoginServices.LoginUser(i_Mode));
                if (!verified)
                {
                    await sr_PageService.DisplayAlert("Note", "Please verify your email to continue!", "OK");
                }
                else
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ModeViewModel)}:commandHelper:{i_Mode}", "Error", e.Message);
            }
        }
        #endregion
    }
}