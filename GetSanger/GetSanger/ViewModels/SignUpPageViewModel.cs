using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(UserJson), "userJson")]
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

        private IList<eCategory> m_CheckedItems;

        private string m_PickedGender;

        private User m_CreatedUser;

        private DateTime m_MaxDatePicker;

        #endregion

        #region Constructor

        public SignUpPageViewModel()
        {
            SetCommands();
            GenderItems = typeof(GenderType).GetListOfEnumNames();
            CategoriesItems = new ObservableCollection<CategoryCell>(typeof(eCategory).GetListOfEnumNames()
                .Select(name => new CategoryCell
                                                {Category = (eCategory) Enum.Parse(typeof(eCategory), name)}).ToList());
            m_TempCategories = CategoriesItems;
            MaxDatePicker = DateTime.Now.AddYears(-18);
        }

        #endregion

        #region Property

        public User CreatedUser
        {
            get => m_CreatedUser;
            set => SetClassProperty(ref m_CreatedUser, value);
        }

        public DateTime MaxDatePicker
        {
            get => m_MaxDatePicker;
            set => SetStructProperty(ref m_MaxDatePicker, value); 
        }

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
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    CreatedUser = ObjectJsonSerializer.DeserializeForPage<User>(value);
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
            sr_CrashlyticsService.LogPageEntrance(nameof(SignUpPageViewModel));
            CreatedUser ??= new User();
            PersonalImage = sr_PhotoDisplay.DisplayPicture(CreatedUser.ProfilePictureUri);
            CreatedUser.PersonalDetails.Birthday = DateTime.Now.AddYears(-18);
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            base.SetCommands();
            EmailPartCommand = new Command(emailPartClicked);
            PersonalDetailPartCommand = new Command(personalDetailPartClicked);
            CategoriesPartCommand = new Command(categoriesPartClicked);
            AllCategoriesCommand = new Command(allCategoriesChecked);
            ImagePickerCommand = new Command(imagePicker);
            BackButtonBehaviorCommand = new Command(backButtonBehavior);
        }

        private async void backButtonBehavior(object i_Param) // when move from email page back to registration page
        {
            try
            {
                if (IsFacebookGmail == false && InPersonalPage == true)
                {
                    await GoBack();
                }
                else
                {
                    PropertyInfo[] properties = GetType()
                        .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        if (property.PropertyType.Equals(typeof(ICommand))
                            || property.Name.Equals(nameof(GenderItems))
                            || property.Name.Equals(nameof(MaxDatePicker)))
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

                        property.SetValue(this, null);
                    }

                    await GoBack();
                }
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(SignUpPageViewModel)}:backButtonBehavior", "Error", e.Message);
            }
        }

        private async void emailPartClicked()
        {
            if (AuthHelper.IsValidEmail(CreatedUser.Email) == false)
            {
                await sr_PageService.DisplayAlert("Notice", "Please enter a valid email address!", "OK");
                return;
            }

            if (Password.Equals(ConfirmPassword))
            {
                try
                {
                    if (Password.IsValidPassword())
                    {
                        await RunTaskWhileLoading(AuthHelper.RegisterViaEmail(CreatedUser.Email, Password));
                        await sr_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={false}");
                    }
                    else
                    {
                        await sr_PageService.DisplayAlert("Note", "Password of at least 6 chars must contain at list: one capital letter, one lower letter, one digit, one special character", "OK");
                    }
                }
                catch (Exception e)
                {
                    await e.LogAndDisplayError($"{nameof(SignUpPageViewModel)}:emailPartClicked", "Notice", e.Message);
                }
            }
            else
            {
                await sr_PageService.DisplayAlert("Notice", "Please check the password is correct", "OK");
            }
        }

        private async void personalDetailPartClicked()
        {
            try
            {
                if (!PersonalDetails.IsValidName(CreatedUser.PersonalDetails.NickName) || !sr_DialService.IsValidPhone(CreatedUser.PersonalDetails.Phone))
                {
                    await sr_PageService.DisplayAlert("Note", "Please fill all the fields in the form!", "OK");
                }

                CreatedUser.PersonalDetails.Gender = (GenderType)Enum.Parse(typeof(GenderType), PickedGender);
                await sr_NavigationService.NavigateTo(ShellRoutes.SignupCategories);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(SignUpPageViewModel)}:personalDetailPartClicked", "Error", e.Message);
            }
        }

        private async void categoriesPartClicked()
        {
            try
            {
                m_CheckedItems = (from category in CategoriesItems
                where category.Checked == true && category.Category.Equals(eCategory.All) == false
                select category.Category).ToList();
                CreatedUser.Categories = new ObservableCollection<eCategory>(m_CheckedItems);
                CreatedUser.RegistrationToken = await sr_PushService.GetRegistrationToken();
                CreatedUser.UserId ??= AuthHelper.GetLoggedInUserId();
                CreatedUser.UserLocation = await sr_LocationService.GetCurrentLocation();
                await RunTaskWhileLoading(FireStoreHelper.AddUser(CreatedUser));
                await RunTaskWhileLoading(sr_PushService.RegisterTopics(CreatedUser.UserId,
                    (m_CheckedItems.Select(category => ((int) category).ToString())).ToArray()));
                await PopupNavigation.Instance.PushAsync(new ModePage());
                await sr_NavigationService.NavigateTo("../../..");
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(SignUpPageViewModel)}:categoriesPartClicked", "Notice", e.Message);
            }
        }

        private async void imagePicker(object i_Param)
        {
            try
            {
                CreatedUser.UserId ??= AuthHelper.GetLoggedInUserId();
                await sr_PhotoDisplay.TryGetPictureFromStream(CreatedUser);
                PersonalImage = sr_PhotoDisplay.DisplayPicture(CreatedUser.ProfilePictureUri);
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(SignUpPageViewModel)}:imagePicker", "Error", "Something went wrong, please try again later");
            }
        }

        private void allCategoriesChecked(object i_Param)
        {
            var category = i_Param as CategoryCell;
            if (category != null)
            {
                if (category.Category.Equals(eCategory.All))
                {
                    foreach (var elem in m_TempCategories)
                    {
                        elem.Checked = category.Checked;
                    }

                    CategoriesItems = new ObservableCollection<CategoryCell>(m_TempCategories);
                }
            }
        }

        #endregion
    }
}