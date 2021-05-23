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

        public ICommand FaceBookLoginCommand { get; set; }

        public ICommand GmailLoginCommand { get; set; }

        #endregion

        #region methods

        public override void Appearing()
        {
            // auto log in if connected
        }

        private void setCommands()
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
            this.ForgotPasswordCommand = new Command(this.ForgotPasswordClicked);
            this.FaceBookLoginCommand = new Command(this.FaceBookClicked);
            this.GmailLoginCommand = new Command(this.GmailClicked);
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

        private async void FaceBookClicked(object obj)
        {
            Dictionary<string, string> details = await RunTaskWhileLoading(AuthHelper.LoginViaFacebook());
            bool isFirstLoggedin = await RunTaskWhileLoading(AuthHelper.IsFirstTimeLogIn());
            if (isFirstLoggedin)
            {
                await r_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={true}");
            }

            r_LoginServices.LoginUser();
            // need to check if it is not the first time
        }

        private async void GmailClicked(object obj)
        {
            Dictionary<string, object> details = await RunTaskWhileLoading(AuthHelper.LoginViaGoogle());
            bool isFirstLoggedin = await RunTaskWhileLoading(AuthHelper.IsFirstTimeLogIn());
            if (isFirstLoggedin)
            {
                string user = JsonSerializer.Serialize(getGmailDetails(details));
                string route = string.Format(@"{0}?userJson={1}&isFacebookGmail={2}", ShellRoutes.SignupPersonalDetails, user, true);
                await r_NavigationService.NavigateTo(route);
            }
            else
            {
                r_LoginServices.LoginUser();
                // need to check if it is not the first time
            }
        }

        private User getGmailDetails(Dictionary<string, object> i_Details)
        {
            User user = new User
            {
                PersonalDetails = new PersonalDetails
                {
                    NickName = i_Details["displayName"] as string
                },
                Email = i_Details["email"] as string,
            };

            if(i_Details["photoUrl"] != null)
            {
                user.ProfilePictureUri = new Uri(i_Details["photoUrl"] as string);
            }

            return user;
        }

        #endregion
    }
}