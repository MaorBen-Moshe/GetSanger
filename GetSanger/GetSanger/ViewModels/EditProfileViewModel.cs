using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        #region Fields
        private ImageSource m_ProfileImage;
        private string m_NickName;
        private DateTime m_Birthday;
        private GenderType m_Gender;
        private ObservableCollection<GenderType> m_GenderItems;
        private ContactPhone m_Phone;
        private bool m_DataChanged;
        #endregion

        #region Properties
        private User ConnectedUser { get; set; } 

        public ImageSource ProfileImage
        {
            get => m_ProfileImage;
            set
            {
                m_DataChanged = true;
                SetClassProperty(ref m_ProfileImage, value);
            }
        }

        public string NickName
        {
            get => m_NickName;
            set
            {
                m_DataChanged = true;
                SetClassProperty(ref m_NickName, value);
            }
        }

        public DateTime Birthday
        {
            get => m_Birthday;
            set
            {
                m_DataChanged = true;
                SetStructProperty(ref m_Birthday, value);
            }
        }

        public GenderType Gender
        {
            get => m_Gender;
            set
            {
                m_DataChanged = true;
                SetStructProperty(ref m_Gender, value);
            }
        }

        public ObservableCollection<GenderType> GenderItems
        {
            get => m_GenderItems;
            set => SetClassProperty(ref m_GenderItems, value);
        }

        public ContactPhone Phone
        {
            get => m_Phone;
            set
            {
                m_DataChanged = true;
                SetClassProperty(ref m_Phone, value);
            }
        }
        #endregion

        #region Commands
        public ICommand ImageChosenCommand { get; set; }
        public ICommand BackButtonCommand { get; set; }
        public ICommand AppearingPageCommand { get; set; }
        public ICommand DisappearingPageCommand { get; set; }
        #endregion

        #region Constructor
        public EditProfileViewModel()
        {
            ImageChosenCommand = new Command(imageChanged);
            BackButtonCommand = new Command(backButtonBehavior);
            AppearingPageCommand = new Command(appearing);
            DisappearingPageCommand = new Command(disappearing);
        }

        #endregion

        #region Methods
        private void initialData()
        {
            ConnectedUser = AppManager.Instance.ConnectedUser;
            NickName = ConnectedUser.PersonalDetails.Nickname;
            Birthday = ConnectedUser.PersonalDetails.Birthday;
            Gender = ConnectedUser.PersonalDetails.Gender;
            Phone = ConnectedUser.PersonalDetails.Phone;
            ProfileImage = ImageSource.FromUri(ConnectedUser.ProfilePictureUri);
            m_DataChanged = false;
        }

        private async void backButtonBehavior(object i_Param)
        {
            if (m_DataChanged)
            {
                bool answer = await r_PageService.DisplayAlert("Note", "Do you want to save data?", "Yes", "No");
                if (answer)
                {
                    // check for empty entries
                    ConnectedUser.PersonalDetails.Nickname = NickName;
                    ConnectedUser.PersonalDetails.Birthday = Birthday;
                    ConnectedUser.PersonalDetails.Gender = Gender;
                    ConnectedUser.PersonalDetails.Phone = Phone;
                    //ConnectedUser.ProfilePictureUri = 
                    await RunTaskWhileLoading(FireStoreHelper.UpdateUser(ConnectedUser));
                }
            }

            await GoBack();
        }

        private async void imageChanged(object i_Param)
        {
            Stream stream = await DependencyService.Get<IPhotoPicker>().GetImageStreamAsync();
            ProfileImage = ImageSource.FromStream(() => stream);
        }

        protected void appearing(object i_Param)
        {
            initialData();
            GenderItems = new ObservableCollection<GenderType>(AppManager.Instance.GetListOfEnumNames(typeof(GenderType)).Select(name => (GenderType)Enum.Parse(typeof(GenderType), name)).ToList());
        }

        protected void disappearing(object i_Param)
        {
        }
        #endregion
    }
}
