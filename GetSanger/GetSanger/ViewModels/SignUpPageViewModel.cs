using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SignUpPageViewModel : LoginViewModel
    {
        #region Fields

        private string m_Name;

        private string m_Password;

        private string m_ConfirmPassword;

        private string m_PhoneNumber;

        private Image m_PersonalImage;

        private DateTime m_Birthday;

        private IList<string> m_GenderItems;

        private ObservableCollection<CategoryCell> m_CategoriesItems;

        private IList<CategoryCell> m_TempCategories;

        private IList<Category> m_CheckedItems;

        private string m_PickedGender;

        #endregion

        #region Constructor

        public SignUpPageViewModel()
        {
            EmailPartCommand = new Command(emailPartClicked);
            PersonalDetailPartCommand = new Command(personalDetailPartClicked);
            CategoriesPartCommand = new Command(categoriesPartClicked);
            AllCategoriesCommand = new Command(allCategoriesChecked);
            ImagePickerCommand = new Command(imagePicker);
            Birthday = DateTime.Now;
            //GenderItems = (from action in (GenderType[])Enum.GetValues(typeof(GenderType)) select action.ToString()).ToList();
            GenderItems = AppManager.Instance.GetListOfEnumNames(typeof(GenderType));
            CategoriesItems = new ObservableCollection<CategoryCell>(AppManager.Instance.GetListOfEnumNames(typeof(Category)).Select(name => new CategoryCell { Category = (Category)Enum.Parse(typeof(Category), name) }).ToList());
            m_TempCategories = CategoriesItems;
        }

        #endregion

        #region Property

        public string Name
        {
            get => m_Name;
            set => SetClassProperty(ref m_Name, value);
        }

        public new string Password
        {
            get => m_Password;
            set => SetClassProperty(ref m_Password, value);
        }

        public string ConfirmPassword
        {
            get => m_ConfirmPassword;
            set => SetClassProperty(ref m_ConfirmPassword, value);
        }

        public string PhoneNumber
        {
            get => m_PhoneNumber;
            set => SetClassProperty(ref m_PhoneNumber, value);
        }

        public Image PersonalImage
        {
            get => m_PersonalImage;
            set => SetClassProperty(ref m_PersonalImage, value);
        }

        public DateTime Birthday
        {
            get => m_Birthday;
            set => SetStructProperty(ref m_Birthday, value);
        }

        public IList<string> GenderItems
        {
            get => m_GenderItems;
            set => SetClassProperty(ref m_GenderItems, value);
        }

        public ObservableCollection<CategoryCell> CategoriesItems
        {
            get => m_CategoriesItems;
            set => SetClassProperty(ref m_CategoriesItems, value);
        }

        public string PickedGender
        {
            get => m_PickedGender;
            set => SetClassProperty(ref m_PickedGender, value);
        }

        #endregion

        #region Command

        public ICommand EmailPartCommand { get; set; }

        public ICommand AllCategoriesCommand { get; set; }

        public ICommand PersonalDetailPartCommand { get; set; }

        public ICommand CategoriesPartCommand { get; set; }

        public ICommand ImagePickerCommand { get; set; }

        #endregion

        #region Methods

        private async void emailPartClicked()
        {
            if(AuthHelper.IsValidEmail(Email) == false)
            {
                await r_PageService.DisplayAlert("Notice", "Please enter a valid email address!", "OK");
                return;
            }

            if (Password.Equals(ConfirmPassword))
            {
                // go to next page in sign up
            }

            await r_PageService.DisplayAlert("Notice", "Please check the password is correct", "OK");
        }

        private void personalDetailPartClicked()
        {
            PersonalDetails personal = new PersonalDetails
            {
                Phone = new ContactPhone(PhoneNumber),
                Nickname = Name,
                Gender = (GenderType)Enum.Parse(typeof(GenderType), PickedGender),
                Birthday = Birthday
            };
            // register and move to mode page
        }

        private void categoriesPartClicked()
        {
            m_CheckedItems = (from category in CategoriesItems 
                             where category.Checked == true && category.Category.Equals(Category.All) == false select category.Category).ToList();
            //register user to topic
            // continue next page
        }

        private async void imagePicker(object i_Param)
        {
            (i_Param as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPicker>().GetImageStreamAsync();
            if (stream != null)
            {
                PersonalImage.Source = ImageSource.FromStream(() => stream);
            }
            else
            {
                await r_PageService.DisplayAlert("Error", "Something went wrong, please try again later", "Ok");
            }

            (i_Param as Button).IsEnabled = true;
        }

        private void allCategoriesChecked(object i_Param)
        {
            var category = i_Param as CategoryCell;
            if (category != null && category.Category.Equals(Category.All))
            {
                foreach (var elem in m_TempCategories)
                {
                    elem.Checked = category.Checked;
                }

                CategoriesItems = new ObservableCollection<CategoryCell>(m_TempCategories);
            }
        }

        #endregion
    }
}