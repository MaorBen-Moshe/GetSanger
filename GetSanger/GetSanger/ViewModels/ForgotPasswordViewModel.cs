using GetSanger.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    public class ForgotPasswordViewModel : LoginViewModel
    {
        private string m_Email;
        private string m_ConfirmEmail;
        #region Constructor

        public ForgotPasswordViewModel()
        {
            this.SendCommand = new Command(this.SendClicked);
        }

        #endregion

        #region properties
        public new string Email
        {
            get => m_Email;
            set => SetClassProperty(ref m_Email, value);
        }

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
            if (Email.Equals(ConfirmEmail) == false)
            {
                await r_PageService.DisplayAlert("Error", "Please Check the fields are equal!", "OK");
                return;
            }

            AuthHelper.ForgotPassword(Email);
        }

        #endregion
    }
}