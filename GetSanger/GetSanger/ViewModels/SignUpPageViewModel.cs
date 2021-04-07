using GetSanger.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    /// <summary>
    /// ViewModel for sign-up page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class SignUpPageViewModel : LoginViewModel
    {
        #region Fields

        private string m_Name;

        private string m_Password;

        private string m_ConfirmPassword;

        #endregion

        #region Constructor

        public SignUpPageViewModel()
        {
            EmailPartCommand = new Command(EmailPartClicked);
            ImagePickerCommand = new Command(imagePicker);
        }

        #endregion

        #region Property

        public string Name
        {
            get => m_Name;
            set => SetClassProperty(ref m_Name, value);
        }

        public new string Password
        {
            get => m_Password;
            set => SetClassProperty(ref m_Password, value);
        }

        public string ConfirmPassword
        {
            get => m_ConfirmPassword;
            set => SetClassProperty(ref m_ConfirmPassword, value);
        }

        #endregion

        #region Command

        public ICommand EmailPartCommand { get; set; }

        public ICommand ImagePickerCommand { get; set; }

        #endregion

        #region Methods

        private async void EmailPartClicked()
        {
            if(AuthHelper.IsValidEmail(Email) == false)
            {
                await r_PageService.DisplayAlert("Notice", "Please enter a valid email address!", "OK");
                return;
            }

            if (Password.Equals(ConfirmPassword))
            {
                // go to next page in sign up
            }

            await r_PageService.DisplayAlert("Notice", "Please check the password is correct", "OK");
        }

        private void imagePicker(object i_Param)
        {

        }

        #endregion
    }
}