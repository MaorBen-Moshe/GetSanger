using GetSanger.Models;
using GetSanger.Services;
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
            //CurrentUser = AppManager.Instance.ConnectedUser ?? throw new ArgumentException("User details are not available!");
           // UserImage = ImageSource.FromUri(CurrentUser.ProfilePictureUri);
            EditProfileCommand = new Command(editProfile);
            SettingCommand = new Command(setting);
            ChangeModeCommand = new Command(changeMode);
            LogoutCommand = new Command(logout);
            CurrentUser = new User
            {
                PersonalDetails = new PersonalDetails
                {
                    Nickname = "Maor"
                }
            };
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

        private void setting(object i_Param)
        {
            // go to setiing page
        }

        private void editProfile(object i_Param)
        {
            // go to edit profile page
        }
        #endregion

    }
}
