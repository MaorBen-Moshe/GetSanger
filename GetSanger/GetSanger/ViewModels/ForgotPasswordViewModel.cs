using GetSanger.Extensions;
using GetSanger.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    public class ForgotPasswordViewModel : LoginViewModel
    {
        #region Fields
        private string m_ConfirmEmail;
        #endregion

        #region Constructor

        public ForgotPasswordViewModel()
        {
            SetCommands();
        }

        #endregion

        #region properties
        public string ConfirmEmail
        {
            get => m_ConfirmEmail;
            set => SetClassProperty(ref m_ConfirmEmail, value);
        }

     

        #endregion

        #region Command
        public ICommand SendCommand { get; set; }
        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(ForgotPasswordViewModel));
            base.Appearing();
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            base.SetCommands();
            SendCommand = new Command(this.SendClicked);
        }

        private async void SendClicked()
        {
            try
            {
                if (AuthHelper.IsValidEmail(Email) == false || Email.Equals(ConfirmEmail) == false)
                {
                    await sr_PageService.DisplayAlert("Error", "Please Check your email", "OK");
                    return;
                }

                await RunTaskWhileLoading(AuthHelper.ForgotPassword(Email));
                await sr_PageService.DisplayAlert("Note", "Email has been sent!", "Thanks");
                await PopupNavigation.Instance.PopAsync();
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ForgotPasswordViewModel)}:SendClicked", "Error", e.Message);
            }
        }

        #endregion
    }
}