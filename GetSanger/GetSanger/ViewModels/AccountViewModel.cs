using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.IO;
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

        public ICommand MyRatingsCommand { get; set; }

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
            MyRatingsCommand = new Command(myRatings);
        }

        private async void initialPage()
        {
            CurrentUser = null;
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
            string labelText = i_Param as string;
            bool isValidMode = Enum.TryParse(labelText, out AppMode mode);
            if (isValidMode)
            {
                Application.Current.MainPage = AppManager.Instance.GetCurrentShell(mode);
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

        private async void linkSocial(object i_Param)
        {
            List<eSocialProvider> providers = AppManager.Instance.GetListOfEnumNames(typeof(eSocialProvider)).Select(item => (eSocialProvider)Enum.Parse(typeof(eSocialProvider), item)).ToList();
            string[] buttons = providers.Select(item => item.ToString()).ToArray();
            string choice = await r_PageService.DisplayActionSheet("Choose Provider", "Cancel", null, buttons);
            if(choice.Equals("Cancel") == false)
            {
                eSocialProvider parsedProvider = (eSocialProvider)Enum.Parse(typeof(eSocialProvider), choice);
                providerSelected(parsedProvider);
            }
        }

        private async void myRatings(object i_Param)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.MyRatings);
        }

        private async void providerSelected(eSocialProvider i_CurrentProvider)
        {
            try
            {
                r_LoadingService.ShowPopup();
                if (i_CurrentProvider.Equals(eSocialProvider.Email))
                {
                    await r_NavigationService.NavigateTo(ShellRoutes.LinkEmail);
                    return;
                }

                Dictionary<string, object> details = await AuthHelper.LinkWithSocialProvider(i_CurrentProvider);
                string photoUrl = details.ContainsKey("photoUrl") ? details["photoUrl"] as string : null;

                await r_PhotoDisplay.TryGetPictureFromUri(photoUrl, AppManager.Instance.ConnectedUser);
                await r_PageService.DisplayAlert("Note", $"Your account linked with: {i_CurrentProvider}", "Thanks");
            }
            catch (Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                r_LoadingService.HidePopup();
            }
        }

        #endregion
    }
}
