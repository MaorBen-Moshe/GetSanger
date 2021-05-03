using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        #region Fields
        private User m_CurrentUser;

        private ImageSource m_UserImage;
        #endregion

        #region Properties
        public User CurrentUser
        {
            get => m_CurrentUser;
            set => SetClassProperty(ref m_CurrentUser, value);
        }

        public ImageSource UserImage
        {
            get => m_UserImage;
            set => SetClassProperty(ref m_UserImage, value);
        }
        #endregion

        #region Commands
        public ICommand EditProfileCommand { get; set; }

        public ICommand SettingCommand { get; set; }

        public ICommand ChangeModeCommand { get; set; }

        public ICommand LogoutCommand { get; set; }
        #endregion

        #region Constructor
        public AccountViewModel()
        {
            EditProfileCommand = new Command(editProfile);
            SettingCommand = new Command(setting);
            ChangeModeCommand = new Command(changeMode);
            LogoutCommand = new Command(logout);
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            CurrentUser = AppManager.Instance.ConnectedUser ?? throw new ArgumentException("User details are not available!");
            UserImage = ImageSource.FromUri(CurrentUser.ProfilePictureUri);
            if(UserImage == null)
            {
                UserImage = ImageSource.FromFile("profile.jpg");
            }
        }

        private void logout(object i_Param)
        {
            // do logout

            Shell.Current.GoToAsync("AuthShell");
        }

        private void changeMode(object i_Param)
        {
            // may be in the shell code;
            //AppManager.Instance.CurrentMode = AppManager.Instance.CurrentMode.Equals(AppMode.Client) ? AppMode.Sanger : AppMode.Client;
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Client))
            {
                Application.Current.MainPage = new SangerShell();
            }
            else
            {
                Application.Current.MainPage = new UserShell();
            }
        }

        private async void setting(object i_Param)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.Settings);
        }

        private async void editProfile(object i_Param)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.EditProfile);
        }
        #endregion
    }
}
