using GetSanger.Services;
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
            ChangePasswordCommand = new Command(changePassword);
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
        }

        private async void changePassword(object i_Param)
        {
            bool isUserOldPassword = await AuthHelper.IsUserPassword(OldPassword);
            if (isUserOldPassword)
            {
                // also need to check if the new password follow the convention of right password.
                if (NewPassword.Equals(ConfirmPassword))
                {
                    try
                    {
                        await AuthHelper.ChangePassword(OldPassword, NewPassword);
                        await r_PageService.DisplayAlert("Success", "Password has changed successfully.", "Thanks");
                        await GoBack();
                    }
                    catch
                    {
                        await r_PageService.DisplayAlert("Note", "Please check if you wrote the write confirm password", "OK");
                    }
                }

                await r_PageService.DisplayAlert("Note", "Please check if you wrote the write confirm password", "OK");
                return;
            }

            await r_PageService.DisplayAlert("Error", "You insert wrong current Password. \n Please try again.", "OK");
        }
        #endregion
    }
}
