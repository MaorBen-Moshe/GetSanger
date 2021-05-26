using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        #region Fields
        private ImageSource m_ProfileImage;
        private IList<GenderType> m_GenderItems;
        private User m_ConnectedUser;
        private string m_ClonedUserData;
        private bool m_ValidInput;
        #endregion

        #region Properties
        public User ConnectedUser
        {
            get => m_ConnectedUser;
            set => SetClassProperty(ref m_ConnectedUser, value);
        }

        public bool ValidInput
        {
            get => m_ValidInput;
            set => SetStructProperty(ref m_ValidInput, value);
        }


        public ImageSource ProfileImage
        {
            get => m_ProfileImage;
            set => SetClassProperty(ref m_ProfileImage, value);
        }

        public IList<GenderType> GenderItems
        {
            get => m_GenderItems;
            set => SetClassProperty(ref m_GenderItems, value);
        }
        #endregion

        #region Commands
        public ICommand ImageChosenCommand { get; set; }
        public ICommand BackButtonCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }
        #endregion

        #region Constructor
        public EditProfileViewModel()
        {
            setCommands();
            GenderItems = new ObservableCollection<GenderType>(AppManager.Instance.GetListOfEnumNames(typeof(GenderType)).Select(name => (GenderType)Enum.Parse(typeof(GenderType), name)).ToList());
        }

        #endregion

        #region Methods
        public override void Appearing()
        {
           initialData();
        }

        private void setCommands()
        {
            ImageChosenCommand = new Command(imageChanged);
            BackButtonCommand = new Command(backButtonBehavior);
            ChangePasswordCommand = new Command(changePassword);
            DeleteAccountCommand = new Command(deleteAccount);
        }

        private void initialData()
        {
            ConnectedUser = AppManager.Instance.ConnectedUser;
            m_ClonedUserData = ObjectJsonSerializer.SerializeForPage(ConnectedUser);
            ProfileImage = r_PhotoDisplay.DisplayPicture(ConnectedUser.ProfilePictureUri);
        }

        private async void backButtonBehavior(object i_Param)
        {
            if(string.IsNullOrWhiteSpace(ConnectedUser.PersonalDetails.NickName) || 
                !r_DialService.IsValidPhone(ConnectedUser.PersonalDetails.Phone) ||
                (DateTime.Now.AddYears(-ConnectedUser.PersonalDetails.Birthday.Year).Year >= 0)
                )
            {
                await r_PageService.DisplayAlert("Note", "Not all of your data contains valid data.\n Data remain the same!", "OK");
                AppManager.Instance.ConnectedUser = ObjectJsonSerializer.DeserializeForPage<User>(m_ClonedUserData); // delete the new changes
            }
            else
            {
                await RunTaskWhileLoading(FireStoreHelper.UpdateUser(ConnectedUser));
            }

            await GoBack();
        }

        private async void imageChanged(object i_Param)
        {
            Stream stream = await DependencyService.Get<IPhotoPicker>().GetImageStreamAsync();
            if(stream == null)
            {
                ProfileImage = r_PhotoDisplay.DisplayPicture();
                r_StorageHelper.DeleteProfileImage(ConnectedUser.UserId);
                ConnectedUser.ProfilePictureUri = null;
                return;
            }

            ProfileImage = ImageSource.FromStream(() => stream);
            r_StorageHelper.SetUserProfileImage(ConnectedUser, stream);
        }

        private async void changePassword(object i_Param)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.ChangePassword);
        }

        private async void deleteAccount()
        {
            bool answer = await r_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No");
            if (answer)
            {
                await RunTaskWhileLoading(FireStoreHelper.DeleteUser(AppManager.Instance.ConnectedUser.UserId));
                //do delete
                await r_PageService.DisplayAlert("Note", "We hope you come back soon!", "Thanks!");
                Application.Current.MainPage = new AuthShell();
            }
        }
        #endregion
    }
}
