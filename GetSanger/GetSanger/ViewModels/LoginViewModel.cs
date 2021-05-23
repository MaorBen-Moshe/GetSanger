using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.Registration;
using System;
using System.Collections.Generic;
using System.Text.Json;
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
        private bool m_IsIOSDevice;

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

        public bool isIOSDevice
        {
            get => m_IsIOSDevice;
            set => SetStructProperty(ref m_IsIOSDevice, value);
        }

        #endregion

        #region Command

        public ICommand LoginCommand { get; set; }

        public ICommand SignUpCommand { get; set; }

        public ICommand ForgotPasswordCommand { get; set; }

        public ICommand FaceBookLoginCommand { get; set; }

        public ICommand GmailLoginCommand { get; set; }

        public ICommand AppleLoginCommand { get; set; }

        #endregion

        #region methods

        public override void Appearing()
        {
            isIOSDevice = Device.RuntimePlatform.Equals("iOS");
            // auto log in if connected
        }

        private void setCommands()
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
            this.ForgotPasswordCommand = new Command(this.ForgotPasswordClicked);
            this.FaceBookLoginCommand = new Command(this.FacebookClicked);
            this.GmailLoginCommand = new Command(this.GmailClicked);
            this.AppleLoginCommand = new Command(this.AppleClicked);
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
                r_LoginServices.LoginUser();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
        }

        protected async void SignUpClicked()
        {
            await r_NavigationService.NavigateTo(ShellRoutes.SignupEmail);
        }

        private async void ForgotPasswordClicked(object obj)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.ForgotPassword);
        }

        private void AppleClicked(object obj)
        {
            r_SocialService.SocialLogin(SocialProvider.Apple);
        }

        private void FacebookClicked(object obj)
        {
            r_SocialService.SocialLogin(SocialProvider.Facebook);
        }

        private void GmailClicked(object obj)
        {
            r_SocialService.SocialLogin(SocialProvider.Gmail);
        }

        #endregion
    }
}