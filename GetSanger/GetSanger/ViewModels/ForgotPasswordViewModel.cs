using GetSanger.Services;
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
            setCommands();
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
        private void setCommands()
        {
            SendCommand = new Command(this.SendClicked);
        }

        private async void SendClicked()
        {
            if (AuthHelper.IsValidEmail(Email) == false || Email.Equals(ConfirmEmail) == false)
            {
                await r_PageService.DisplayAlert("Error", "Please Check your email", "OK");
                return;
            }

            await RunTaskWhileLoading(AuthHelper.ForgotPassword(Email));
        }

        #endregion
    }
}