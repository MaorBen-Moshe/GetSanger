using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    [QueryProperty(nameof(IsFacebookGamil), "isFacebookGamail")]
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

        private User m_CreatedUser;

        #endregion

        #region Constructor

        public SignUpPageViewModel()
        {
            setCommands();
            Birthday = DateTime.Now.Date.ToUniversalTime();
            GenderItems = AppManager.Instance.GetListOfEnumNames(typeof(GenderType));
            CategoriesItems = new ObservableCollection<CategoryCell>(AppManager.Instance
                .GetListOfEnumNames(typeof(Category)).Select(name => new CategoryCell
                    {Category = (Category) Enum.Parse(typeof(Category), name)}).ToList());
            m_TempCategories = CategoriesItems;
        }

        #endregion

        #region Property

        public bool IsFacebookGamil { get; set; }

        public string Name
        {
            get => m_Name;
            set => SetClassProperty(ref m_Name, value);
        }

        public string UserId { get; set; }

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

        public ICommand BackButtonBehaviorCommand { get; set; }

        public ICommand EmailPartCommand { get; set; }

        public ICommand AllCategoriesCommand { get; set; }

        public ICommand PersonalDetailPartCommand { get; set; }

        public ICommand CategoriesPartCommand { get; set; }

        public ICommand ImagePickerCommand { get; set; }

        #endregion

        #region Methods

        private void setCommands()
        {
            EmailPartCommand = new Command(emailPartClicked);
            PersonalDetailPartCommand = new Command(personalDetailPartClicked);
            CategoriesPartCommand = new Command(categoriesPartClicked);
            AllCategoriesCommand = new Command(allCategoriesChecked);
            ImagePickerCommand = new Command(imagePicker);
            BackButtonBehaviorCommand = new Command(backButtonBehavior);
        }

        private async void backButtonBehavior(object i_Param) // when move from email page back to registration page
        {
            bool answer =
                await r_PageService.DisplayAlert("Warning", "Are you sure?\n any detail will be lost.", "Yes", "No");
            if (answer)
            {
                PropertyInfo[] properties = GetType()
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                Email = null; // property of base class
                foreach (var property in properties)
                {
                    if (property.PropertyType.Equals(typeof(ICommand)) || property.Name.Equals(nameof(GenderItems)))
                    {
                        continue;
                    }

                    else if (IsFacebookGamil && (property.Name.Equals(nameof(Email)) ||
                                                 property.Name.Equals(nameof(Password)) ||
                                                 property.Name.Equals(nameof(ConfirmPassword))))
                    {
                        continue;
                    }

                    else if (property.Name.Equals(nameof(CategoriesItems)))
                    {
                        foreach (var cell in CategoriesItems)
                        {
                            cell.Checked = false;
                        }

                        continue;
                    }

                    else if (property.Name.Equals(nameof(Birthday)))
                    {
                        property.SetValue(this, DateTime.Now);
                        continue;
                    }

                    property.SetValue(this, null);
                }

                await GoBack();
            }
        }

        private async void emailPartClicked()
        {
            if (AuthHelper.IsValidEmail(Email) == false)
            {
                await r_PageService.DisplayAlert("Notice", "Please enter a valid email address!", "OK");
                return;
            }

            if (Password.Equals(ConfirmPassword))
            {
                try
                {
                    //await RunTaskWhileLoading(AuthHelper.RegisterViaEmail(Email, Password));
                    await AuthHelper.RegisterViaEmail(Email, Password);
                    m_CreatedUser = new User
                    {
                        UserID = AuthHelper.GetLoggedInUserId()
                    };

                    await r_NavigationService.NavigateTo(
                        ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={false}");
                }
                catch (Exception e)
                {
                    await r_PageService.DisplayAlert("Notice", e.Message, "OK");
                }
            }
            else
            {
                await r_PageService.DisplayAlert("Notice", "Please check the password is correct", "OK");
            }
        }

        private async void personalDetailPartClicked()
        {
            PersonalDetails personal = new PersonalDetails
            {
                Phone = new ContactPhone(PhoneNumber),
                Nickname = Name,
                Gender = (GenderType) Enum.Parse(typeof(GenderType), PickedGender),
                Birthday = Birthday
            };
            //  need to set the image also
            // need to set the user location
            m_CreatedUser.PersonalDetails = personal;
            await r_NavigationService.NavigateTo(ShellRoutes.SignupCategories);
        }

        private async void categoriesPartClicked()
        {
            m_CheckedItems = (from category in CategoriesItems
                where category.Checked == true && category.Category.Equals(Category.All) == false
                select category.Category).ToList();

            m_CreatedUser.Categories = (List<Category>) m_CheckedItems;
            m_CreatedUser.RegistrationToken = await r_PushService.GetRegistrationToken();

            try
            {
                //await RunTaskWhileLoading(FireStoreHelper.AddUser(m_CreatedUser));
                await FireStoreHelper.AddUser(m_CreatedUser);
                //await r_PushService.RegisterTopics(UserId,
                //    (m_CheckedItems.Select(category => category.ToString())).ToArray());
                await r_NavigationService.NavigateTo(ShellRoutes.ModePage);
            }
            catch (Exception e)
            {
                await r_PageService.DisplayAlert("Notice", e.Message, "OK");
            }
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