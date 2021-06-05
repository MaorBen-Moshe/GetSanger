using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        #region Fields
        private ObservableCollection<CategoryCell> m_CategoriesItems;
        private bool m_IsGenericNotificatons;
        private List<string> m_NewCategoriesSubscribed;
        private List<string> m_NewCategoriesUnsubscribed;
        #endregion

        #region Properties
        public ObservableCollection<CategoryCell> CategoriesItems
        {
            get => m_CategoriesItems;
            set => SetClassProperty(ref m_CategoriesItems, value);
        }

        public bool IsGenericNotificatons
        {
            get => m_IsGenericNotificatons;
            set => SetStructProperty(ref m_IsGenericNotificatons, value);
        }
        #endregion

        #region Commands
        public ICommand ToggledCommand { get; set; }

        public ICommand BackButtonCommand { get; set; }

        #endregion

        #region Constructor
        public SettingViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            CategoriesItems = new ObservableCollection<CategoryCell>(
            (from
                category in AppManager.Instance.GetListOfEnumNames(typeof(Category))
             where
                !category.Equals(Category.All.ToString())
             select
                new CategoryCell
                {
                    Category = (Category)Enum.Parse(typeof(Category), category),
                    Checked = AppManager.Instance.ConnectedUser.Categories.Contains((Category)Enum.Parse(typeof(Category), category))
                }
            ).ToList());
            IsGenericNotificatons = AppManager.Instance.ConnectedUser.IsGenericNotifications;
        }

        public void Disappearing()
        {
            if(m_NewCategoriesSubscribed != null && m_NewCategoriesUnsubscribed != null)
            {
                BackButtonCommand.Execute(null);
            }
        }

        private void setCommands()
        {
            ToggledCommand = new Command(toggled);
            BackButtonCommand = new Command(backButtonBehavior);
        }

        private async void backButtonBehavior()
        {
            if(m_NewCategoriesSubscribed.Count > 0)
            {
                await RunTaskWhileLoading(r_PushService.RegisterTopics(AppManager.Instance.ConnectedUser.UserId, m_NewCategoriesSubscribed.ToArray()));
            }
            if(m_NewCategoriesUnsubscribed.Count > 0)
            {
                await RunTaskWhileLoading(r_PushService.UnsubscribeTopics(AppManager.Instance.ConnectedUser.UserId, m_NewCategoriesUnsubscribed.ToArray()));
            }

            await RunTaskWhileLoading(FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser));
            m_NewCategoriesSubscribed = m_NewCategoriesUnsubscribed = null;
            await GoBack();
        }

        private async void toggled(object i_Param)
        {
            m_NewCategoriesSubscribed ??= new List<string>();
            m_NewCategoriesUnsubscribed ??= new List<string>();
            if (i_Param is CategoryCell)
            {
                CategoryCell current = i_Param as CategoryCell;
                if (current.Checked)
                {
                    AppManager.Instance.ConnectedUser.Categories.Add(current.Category);
                    string categoryNumber = ((int)current.Category).ToString();
                    m_NewCategoriesUnsubscribed.Remove(categoryNumber);
                    if (m_NewCategoriesSubscribed.Contains(categoryNumber) == false)
                    {
                        m_NewCategoriesSubscribed.Add(((int)current.Category).ToString());
                    }
                }
                else
                {
                    AppManager.Instance.ConnectedUser.Categories.Remove(current.Category);
                    string categoryNumber = ((int)current.Category).ToString();
                    m_NewCategoriesSubscribed.Remove(categoryNumber);
                    if (m_NewCategoriesUnsubscribed.Contains(categoryNumber) == false)
                    {
                        m_NewCategoriesUnsubscribed.Add(((int)current.Category).ToString());
                    }
                }
            }
            else // generic notifications
            {
                if (AppManager.Instance.ConnectedUser.IsGenericNotifications.Equals(IsGenericNotificatons))
                {
                    return;
                }

                AppManager.Instance.ConnectedUser.IsGenericNotifications = IsGenericNotificatons;
                if (IsGenericNotificatons)
                {
                    await RunTaskWhileLoading(r_PushService.RegisterTopics(AppManager.Instance.ConnectedUser.UserId, Constants.Constants.GenericNotificationTopic));
                }
                else
                {
                    await RunTaskWhileLoading(r_PushService.UnsubscribeTopics(AppManager.Instance.ConnectedUser.UserId, Constants.Constants.GenericNotificationTopic));
                }
            }
        }

        #endregion
    }
}
