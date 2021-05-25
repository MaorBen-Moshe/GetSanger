using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ICommand LinkSocialCommand { get; set; }

        #endregion

        #region Constructor
        public AccountViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            initialPage();   
        }

        private void setCommands()
        {
            EditProfileCommand = new Command(editProfile);
            SettingCommand = new Command(setting);
            ChangeModeCommand = new Command(changeMode);
            LogoutCommand = new Command(logout);
            LinkSocialCommand = new Command(linkSocial);
        }

        private async void initialPage()
        {
            if (AppManager.Instance.ConnectedUser == null)
            {
                try
                {
                    AppManager.Instance.ConnectedUser = await FireStoreHelper.GetUser(AuthHelper.GetLoggedInUserId());
                }
                catch
                {
                    await r_PageService.DisplayAlert("Error", "Something went wrong.\nPlease contact us!", "OK");
                    Application.Current.MainPage = new AuthShell();
                }
            }

            CurrentUser = AppManager.Instance.ConnectedUser;
            UserImage = r_PhotoDisplay.DisplayPicture(CurrentUser.ProfilePictureUri);
        }

        

        private void logout(object i_Param)
        {
            // do logout
            AuthHelper.SignOut();
            Application.Current.MainPage = new AuthShell();
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

        private void linkSocial(object i_Param)
        {
            r_PopupService.ShowPopup(new LinkProviderPage());
        }

        #endregion
    }
}
