using GetSanger.Extensions;
using GetSanger.Services;
using Rg.Plugins.Popup.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        #region Fields
        private string m_OldPassword;
        private string m_NewPassword;
        private string m_ConfirmPassword;
        #endregion

        #region Properties
        public string OldPassword
        {
            get => m_OldPassword;
            set => SetClassProperty(ref m_OldPassword, value);
        }

        public string NewPassword
        {
            get => m_NewPassword;
            set => SetClassProperty(ref m_NewPassword, value);
        }

        public string ConfirmPassword
        {
            get => m_ConfirmPassword;
            set => SetClassProperty(ref m_ConfirmPassword, value);
        }
        #endregion

        #region Commands
        public ICommand ChangePasswordCommand { get; set; }
        #endregion

        #region Constructor
        public ChangePasswordViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(ChangePasswordViewModel));
        }

        private void setCommands()
        {
            ChangePasswordCommand = new Command(changePassword);
        }

        private async void changePassword(object i_Param)
        {
            bool isUserOldPassword = await AuthHelper.IsUserPassword(OldPassword);
            if (isUserOldPassword)
            {
                if (NewPassword.Equals(ConfirmPassword))
                {
                    try
                    {
                        if (NewPassword.IsValidPassword())
                        {
                            await RunTaskWhileLoading(AuthHelper.ChangePassword(OldPassword, NewPassword));
                            await r_PageService.DisplayAlert("Success", "Password has changed successfully.", "Thanks");
                            await PopupNavigation.Instance.PopAsync();
                            return;
                        }
                        else
                        {
                            await r_PageService.DisplayAlert("Note", "Password of at least 6 chars must contain at list: one capital letter, one lower letter, one digit, one special character" , "OK");
                        }
                    }
                    catch
                    {
                        await r_PageService.DisplayAlert("Note", "Please check if you wrote the write confirm password", "OK");
                    }
                }
                else
                {
                    await r_PageService.DisplayAlert("Note", "Please check if you wrote the write confirm password", "OK");
                }
            }
            else
            {
                await r_PageService.DisplayAlert("Error", "You insert wrong current Password. \n Please try again.", "OK");
            }
        }
        #endregion
    }
}
