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
        private void logout(object i_Param)
        {
            // do logout

            Shell.Current.GoToAsync("AuthShell");
        }

        private void changeMode(object i_Param)
        {
            string goTo = AppManager.Instance.CurrentMode.Equals(AppMode.Client) ? "SangerShell" : "UserShell";
            Shell.Current.GoToAsync(goTo);
        }

        private async void setting(object i_Param)
        {
            await Shell.Current.GoToAsync($"/settings");
        }

        private async void editProfile(object i_Param)
        {
            await Shell.Current.GoToAsync($"/editProfile");
        }

        public override void Appearing()
        {
            CurrentUser = AppManager.Instance.ConnectedUser ?? throw new ArgumentException("User details are not available!");
            UserImage = ImageSource.FromUri(CurrentUser.ProfilePictureUri);
        }
        #endregion

    }
}
