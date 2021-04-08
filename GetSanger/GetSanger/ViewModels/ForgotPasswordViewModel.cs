using GetSanger.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    public class ForgotPasswordViewModel : LoginViewModel
    {
        private string m_ConfirmEmail;
        #region Constructor

        public ForgotPasswordViewModel()
        {
            this.SendCommand = new Command(this.SendClicked);
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
        public Command SendCommand { get; set; }
        #endregion

        #region Methods
        private async void SendClicked()
        {
            if (AuthHelper.IsValidEmail(Email) == false || Email.Equals(ConfirmEmail) == false)
            {
                await r_PageService.DisplayAlert("Error", "Please Check your email", "OK");
                return;
            }

            AuthHelper.ForgotPassword(Email);
        }

        #endregion
    }
}