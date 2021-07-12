using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Fields

        private string m_Email;
        private string m_Password;

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            SetCommands();
        }

        #endregion

        #region Property

        public string Email
        {
            get => m_Email;
            set => SetClassProperty(ref m_Email, value);
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

        public ICommand SocialLoginCommand { get; set; }

        #endregion

        #region methods

        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(LoginViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
            this.ForgotPasswordCommand = new Command(this.ForgotPasswordClicked);
            this.SocialLoginCommand = new Command(this.socialClicked);
        }

        private async void LoginClicked(object obj)
        {
            if (Email == null || Password == null)
            {
                await r_PageService.DisplayAlert("Error", "Please enter valid values.", "OK");
                return;
            }

            try
            {
                r_LoadingService.ShowLoadingPage();
                await AuthHelper.LoginViaEmail(Email, Password);
                bool verified = await r_LoginServices.LoginUser();
                if (!verified)
                {
                    await r_PageService.DisplayAlert("Note", "Please verify your email to continue!", "OK");
                }

                r_LoadingService.HideLoadingPage();
            }
            catch (Exception e)
            {
                r_LoadingService.HideLoadingPage();
                await e.LogAndDisplayError($"{nameof(LoginViewModel)}:LoginClicked", "Error", e.Message);
            }
        }

        protected async void SignUpClicked()
        {
            try
            {
                await r_NavigationService.NavigateTo(ShellRoutes.SignupEmail);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(LoginViewModel)}:SignUpClicked", "Error", e.Message);
            }
        }

        private async void ForgotPasswordClicked(object obj)
        {
            try
            {
                var page = new ForgotPasswordPage();
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(LoginViewModel)}:ForgotPasswordClicked", "Error", e.Message);
            }
        }

        private async void socialClicked(object i_Param)
        {
            if(i_Param == null || (i_Param is string == false))
            {
                return;
            }

            bool isProvider = Enum.TryParse(i_Param as string, out eSocialProvider provider);
            if (isProvider)
            {
                try
                {
                    bool verified = await RunTaskWhileLoading(r_SocialService.SocialLogin(provider));
                    if (!verified)
                    {
                        await r_PageService.DisplayAlert("Note", "Please verify your email to continue!", "OK");
                    }
                }
                catch(Exception e)
                {
                    await e.LogAndDisplayError($"{nameof(LoginViewModel)}:socialClicked", "Error", e.Message);
                }
            }
        }

        #endregion
    }
}