using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private bool m_ValidInput;
        private DateTime m_MaxDate;

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

        public DateTime MaxDate
        {
            get => m_MaxDate;
            set => SetStructProperty(ref m_MaxDate, value);
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
            GenderItems = new ObservableCollection<GenderType>(AppManager.Instance.GetListOfEnumNames(typeof(GenderType))
                .Select(name => (GenderType) Enum.Parse(typeof(GenderType), name)).ToList());
            MaxDate = DateTime.Now.AddYears(-18);
        }

        #endregion

        #region Methods

        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(EditProfileViewModel));
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
            string json = ObjectJsonSerializer.SerializeForPage(AppManager.Instance.ConnectedUser);
            ConnectedUser = ObjectJsonSerializer.DeserializeForPage<User>(json);
            ProfileImage = r_PhotoDisplay.DisplayPicture(ConnectedUser.ProfilePictureUri);
            MaxDate = DateTime.Now.AddYears(-18);
        }

        private async void backButtonBehavior(object i_Param)
        {
            // if data has not changed do not call update user
            if (string.IsNullOrWhiteSpace(ConnectedUser.PersonalDetails.NickName) ||
                !r_DialService.IsValidPhone(ConnectedUser.PersonalDetails.Phone) ||
                (DateTime.Now.AddYears(-ConnectedUser.PersonalDetails.Birthday.Year).Year < 18)
            )
            {
                await r_PageService.DisplayAlert("Note", "Not all of your data contains valid data.\n Data remain the same!", "OK");
            }
            else
            {
                // if the data has changed we update in the server, else we do nothing
                if (ConnectedUser.PersonalDetails.Equals(AppManager.Instance.ConnectedUser.PersonalDetails) == false)
                {
                    AppManager.Instance.ConnectedUser = ConnectedUser;
                    await RunTaskWhileLoading(FireStoreHelper.UpdateUser(ConnectedUser), "Saving...");
                }
            }

            await GoBack();
        }

        private async void imageChanged(object i_Param)
        {
            await r_PhotoDisplay.TryGetPictureFromStream(ConnectedUser);
            await FireStoreHelper.UpdateUser(ConnectedUser);
            ProfileImage = r_PhotoDisplay.DisplayPicture(ConnectedUser.ProfilePictureUri);
        }

        private async void changePassword(object i_Param)
        {
            var page = new ChangePasswordPage();
            await PopupNavigation.Instance.PushAsync(page);
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