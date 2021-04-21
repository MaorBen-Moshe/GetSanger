using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Services;
using GetSanger.UI_pages.signup;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    public class LoginViewModel : BaseViewModel
    {
        #region Events

        public event Action DisplayInvalidLoginPrompt;

        #endregion

        #region Fields

        private string m_Email;
        private bool m_IsInvalidEmail;
        private string m_Password;

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
            this.ForgotPasswordCommand = new Command(this.ForgotPasswordClicked);
            this.FaceBookLoginCommand = new Command(this.FaceBookClicked);
            this.GmailLoginCommand = new Command(this.GmailClicked);
        }

        #endregion

        #region Property

        public string Email
        {
            get => m_Email;
            set => SetClassProperty(ref m_Email, value);
        }

        public bool IsInvalidEmail
        {
            get => m_IsInvalidEmail;
            set => SetStructProperty(ref m_IsInvalidEmail, value);
        }

        public string Password
        {
            get => m_Password;
            set => SetClassProperty(ref m_Password, value);
        }

        #endregion

        #region Command

        public ICommand LoginCommand { get; set; }

        public ICommand SignUpCommand { get; set; }

        public ICommand ForgotPasswordCommand { get; set; }

        public ICommand FaceBookLoginCommand { get; set; }

        public ICommand GmailLoginCommand { get; set; }

        #endregion

        #region methods

        private async void LoginClicked(object obj)
        {
            if (Email == null || Password == null)
            {
                await r_PageService.DisplayAlert("Error", "Please enter valid values.", "OK");
                return;
            }

            try
            {
                await AuthHelper.LoginViaEmail(Email, Password.ToString());
                await r_PageService.DisplayAlert("Ok", "Log in successful!", "OK");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
            // check if login succeeded
        }

        protected async void SignUpClicked()
        {
            await r_NavigationService.NavigateTo(ShellRoutes.SignupEmail);
        }

        private async void ForgotPasswordClicked(object obj)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.ForgotPassword);
        }

        private async void FaceBookClicked(object obj)
        {
            await AuthHelper.LoginViaFacebook();
            bool isFirstLoggedin = await AuthHelper.IsFirstTimeLogIn();
            if (isFirstLoggedin)
            {
                await r_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={true}");
            }
        }

        private async void GmailClicked(object obj)
        {
            await AuthHelper.LoginViaGoogle();
            bool isFirstLoggedin = await AuthHelper.IsFirstTimeLogIn();
            if (isFirstLoggedin)
            {
                // go to sign up personal details!
                await r_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={true}");
            }
        }

        #endregion
    }
}