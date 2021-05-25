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
using System.Text.Json;
using System.Web;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(IsFacebookGmail), "isFacebookGmail")]
    public class SignUpPageViewModel : LoginViewModel
    {
        #region Fields

        private string m_Password;

        private string m_ConfirmPassword;

        private ImageSource m_PersonalImage;

        private IList<string> m_GenderItems;

        private ObservableCollection<CategoryCell> m_CategoriesItems;

        private IList<CategoryCell> m_TempCategories;

        private IList<Category> m_CheckedItems;

        private string m_PickedGender;

        #endregion

        #region Constructor

        public SignUpPageViewModel()
        {
            setCommands();
            GenderItems = AppManager.Instance.GetListOfEnumNames(typeof(GenderType));
            CategoriesItems = new ObservableCollection<CategoryCell>(AppManager.Instance
                .GetListOfEnumNames(typeof(Category)).Select(name => new CategoryCell
                    {Category = (Category) Enum.Parse(typeof(Category), name)}).ToList());
            m_TempCategories = CategoriesItems;
        }

        #endregion

        #region Property

        public User CreatedUser { get; set; }

        public Dictionary<string, object> FacebookGmailSignDict { get; set; }

        public bool IsFacebookGmail { get; set; }

        public bool InPersonalPage { get; set; }

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

        public ImageSource PersonalImage
        {
            get => m_PersonalImage;
            set => SetClassProperty(ref m_PersonalImage, value);
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

        public string UserJson
        {
            set
            {
                if(string.IsNullOrWhiteSpace(value) == false)
                {
                    CreatedUser = JsonSerializer.Deserialize<User>(value);
                }
            }
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

        public override void Appearing()
        {
            CreatedUser ??= new User();
            PersonalImage = r_PhotoDisplay.DisplayPicture(CreatedUser.ProfilePictureUri);
            CreatedUser.PersonalDetails.Birthday = DateTime.Now.AddYears(-18);
        }

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
            if (IsFacebookGmail == false && InPersonalPage == true)
            {
                await GoBack();
            }
            else
            {
                bool answer =
              await r_PageService.DisplayAlert("Warning", "Are you sure?\n any detail will be lost.", "Yes", "No");
                if (answer)
                {
                    CreatedUser = new User();
                    PropertyInfo[] properties = GetType()
                        .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        if (property.PropertyType.Equals(typeof(ICommand)) || property.Name.Equals(nameof(GenderItems)))
                        {
                            continue;
                        }
                        else if (IsFacebookGmail && (property.Name.Equals(nameof(Email)) ||
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
                        else if (CreatedUser?.ProfilePictureUri != null)
                        {
                            r_StorageHelper.DeleteProfileImage(CreatedUser.UserId);
                        }

                        property.SetValue(this, null);
                    }

                    await GoBack();
                }
            }
        }

        private async void emailPartClicked()
        {
            if (AuthHelper.IsValidEmail(CreatedUser.Email) == false)
            {
                await r_PageService.DisplayAlert("Notice", "Please enter a valid email address!", "OK");
                return;
            }

            if (Password.Equals(ConfirmPassword))
            {
                try
                {
                    await RunTaskWhileLoading(AuthHelper.RegisterViaEmail(CreatedUser.Email, Password));
                    await r_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={false}");
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
            CreatedUser.PersonalDetails.Gender = (GenderType)Enum.Parse(typeof(GenderType), PickedGender);
            CreatedUser.UserLocation = await RunTaskWhileLoading(r_LocationServices.GetCurrentLocation());
            // need to check validation of personal details in user
            await r_NavigationService.NavigateTo(ShellRoutes.SignupCategories);
        }

        private async void categoriesPartClicked()
        {
            m_CheckedItems = (from category in CategoriesItems
                where category.Checked == true && category.Category.Equals(Category.All) == false
                select category.Category).ToList();

            CreatedUser.Categories = new ObservableCollection<Category>(m_CheckedItems);
            CreatedUser.RegistrationToken = await r_PushService.GetRegistrationToken();

            try
            {
                CreatedUser.UserId = AuthHelper.GetLoggedInUserId();
                await RunTaskWhileLoading(FireStoreHelper.AddUser(CreatedUser));
                await RunTaskWhileLoading(r_PushService.RegisterTopics(CreatedUser.UserId,
                    (m_CheckedItems.Select(category => ((int)category).ToString())).ToArray()));
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
            try
            {
                Stream stream = await DependencyService.Get<IPhotoPicker>().GetImageStreamAsync();
                if (stream != null)
                {
                    PersonalImage = ImageSource.FromStream(() => stream);
                    r_StorageHelper.SetUserProfileImage(CreatedUser, stream);
                }

                (i_Param as Button).IsEnabled = true;
            }
            catch(Exception e)
            {
                await r_PageService.DisplayAlert("Error", "Something went wrong, please try again later", "Ok");
                (i_Param as Button).IsEnabled = true;
            }
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