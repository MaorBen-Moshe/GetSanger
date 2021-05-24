using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views;
using System;
using System.Collections.Generic;
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

        public ICommand LinkGoogleCommand { get; set; }

        public ICommand LinkFacebookCommand { get; set; }

        public ICommand LinkAppleCommand { get; set; }

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
            LinkAppleCommand = new Command(linkApple);
            LinkGoogleCommand = new Command(linkGoogle);
            LinkFacebookCommand = new Command(linkFacebook);

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

        private async void linkGoogle(object i_Param)
        {
            try
            {
                Dictionary<string, object> details = await AuthHelper.LinkWithSocialProvider(SocialProvider.Google);
                await r_PageService.DisplayAlert("Note", "Account has linked!", "OK");
            }
            catch (Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
        }

        private async void linkFacebook(object i_Param)
        {
            try
            {
                Dictionary<string, object> details = await AuthHelper.LinkWithSocialProvider(SocialProvider.Facebook);
                await r_PageService.DisplayAlert("Note", "Account has linked!", "OK");
            }
            catch (Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
        }

        private async void linkApple(object i_Param)
        {
            try
            {
                Dictionary<string, object> details = await AuthHelper.LinkWithSocialProvider(SocialProvider.Apple);
                await r_PageService.DisplayAlert("Note", "Account has linked!", "OK");
            }
            catch (Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
        }

        #endregion
    }
}
