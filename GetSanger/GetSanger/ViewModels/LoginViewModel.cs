using GetSanger.Constants;
using GetSanger.Exceptions;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using GetSanger.Views.Registration;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

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
            setCommands();
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
            CrashlyticsService crashlyticsService = (CrashlyticsService) AppManager.Instance.Services.GetService(typeof(CrashlyticsService));
            crashlyticsService.LogPageEntrance(nameof(LoginViewModel));
        }

        private void setCommands()
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
                await RunTaskWhileLoading(AuthHelper.LoginViaEmail(Email, Password));
                bool verified = await RunTaskWhileLoading(r_LoginServices.LoginUser());
                if (!verified)
                {
                    await r_PageService.DisplayAlert("Note", "Please verify your email to continue!", "OK");
                }
            }
            catch (Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
        }

        protected async void SignUpClicked()
        {
            await r_NavigationService.NavigateTo(ShellRoutes.SignupEmail);
        }

        private async void ForgotPasswordClicked(object obj)
        {
            var page = new ForgotPasswordPage();
            await PopupNavigation.Instance.PushAsync(page);
            //await r_NavigationService.NavigateTo(ShellRoutes.ForgotPassword);
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
                    await r_PageService.DisplayAlert("Error", e.Message, "OK");
                }
            }
        }

        #endregion
    }
}