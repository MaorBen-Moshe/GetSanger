using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
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

        public ICommand LinkSocialCommand { get; set; }

        public ICommand MyRatingsCommand { get; set; }

        public ICommand AboutusCommand { get; set; }

        public ICommand TermOfServiceCommand { get; set; }

        public ICommand RateUsCommand { get; set; }

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
            r_CrashlyticsService.LogPageEntrance(nameof(AccountViewModel));
            initialPage();
        }

        public override void Disappearing()
        {
        }

        protected override void setCommands()
        {
            EditProfileCommand = new Command(editProfile);
            SettingCommand = new Command(setting);
            ChangeModeCommand = new Command(changeMode);
            LogoutCommand = new Command(logout);
            LinkSocialCommand = new Command(linkSocial);
            MyRatingsCommand = new Command(myRatings);
            AboutusCommand = new Command(aboutus);
            TermOfServiceCommand = new Command(termOfService);
            RateUsCommand = new Command(rateUs);
        }

        private async void initialPage()
        {
            try
            {
                CurrentUser = null;
                if (AppManager.Instance.ConnectedUser == null)
                {
                    try
                    {
                        AppManager.Instance.ConnectedUser = await FireStoreHelper.GetUser(AuthHelper.GetLoggedInUserId());
                    }
                    catch (Exception e)
                    {
                        await e.LogAndDisplayError($"{nameof(AccountViewModel)}:initialPage", "Error", "Something went wrong.\nPlease contact us!");
                        Application.Current.MainPage = new AuthShell();
                    }
                }

                CurrentUser = AppManager.Instance.ConnectedUser;
                UserImage = r_PhotoDisplay.DisplayPicture(CurrentUser?.ProfilePictureUri);
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:initialPage", "Error", e.Message);
            }
        }


        private async void logout(object i_Param)
        {
            try
            {
                // do logout
                await r_PushService.UnsubscribeUser(AppManager.Instance.ConnectedUser.UserId);
                AuthHelper.SignOut();
                AppManager.Instance.RefreshAppManager();
                Application.Current.MainPage = new AuthShell();
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:logout", "Error", e.Message);
            }
        }

        private async void changeMode(object i_Param)
        {
            try
            {
                string labelText = i_Param as string;
                bool isValidMode = Enum.TryParse(labelText, out eAppMode mode);
                if (isValidMode)
                {
                    Application.Current.MainPage = AppManager.Instance.GetCurrentShell(mode);
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:changeMode", "Error", e.Message);
            }
        }

        private async void setting(object i_Param)
        {
            try
            {
                await r_NavigationService.NavigateTo(ShellRoutes.Settings);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:setting", "Error", e.Message);
            }
        }

        private async void editProfile(object i_Param)
        {
            try
            {
                await r_NavigationService.NavigateTo(ShellRoutes.EditProfile);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:editProfile", "Error", e.Message);
            }
        }

        private async void linkSocial(object i_Param)
        {
            try
            {
                var page = new SocialList();
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:linkSocial", "Error", e.Message);
            }
        }

        private async void myRatings(object i_Param)
        {
            try
            {
                await r_NavigationService.NavigateTo($"{ShellRoutes.Ratings}?isMyRatings={true}&id={AppManager.Instance.ConnectedUser.UserId}");
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:myRatings", "Error", e.Message);
            }
        }

        private async void termOfService(object i_Param)
        {
            try
            {
                await PopupNavigation.Instance.PushAsync(new TermsOfServicePage());
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:termOfService", "Error", e.Message);
            }
        }

        private async void aboutus(object i_Param)
        {
            try
            {
                await PopupNavigation.Instance.PushAsync(new AboutUsPage());
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:aboutus", "Error", e.Message);
            }
        }

        private async void rateUs(object i_Param)
        {
            try
            {
                IAppRating ratingService = DependencyService.Get<IAppRating>();
                ratingService?.RateApp();
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(AccountViewModel)}:rateUs", "Error", e.Message);
            }
        }

        #endregion
    }
}