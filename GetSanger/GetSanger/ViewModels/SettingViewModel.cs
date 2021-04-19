﻿using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        #region Fields
        private ObservableCollection<CategoryCell> m_CategoriesItems;
        private bool m_IsGenericNotificatons;
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
        #endregion

        #region Constructor
        public SettingViewModel()
        {
            ToggledCommand = new Command(toggled);
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
        #endregion

        #region Methods
        private void toggled(object i_Param)
        {
            if (i_Param is CategoryCell)
            {
                CategoryCell current = i_Param as CategoryCell;
                if (current.Checked)
                {
                    AppManager.Instance.ConnectedUser.Categories.Add(current.Category);
                    r_PushService.RegisterTopics(AppManager.Instance.ConnectedUser.UserID, current.Category.ToString());
                }
                else
                {
                    AppManager.Instance.ConnectedUser.Categories.Remove(current.Category);
                    r_PushService.UnsubscribeTopics(AppManager.Instance.ConnectedUser.UserID, current.Category.ToString());
                }
            }
            else // generic notifications
            {
                AppManager.Instance.ConnectedUser.IsGenericNotifications = IsGenericNotificatons;
                if (IsGenericNotificatons)
                {
                    r_PushService.RegisterTopics(AppManager.Instance.ConnectedUser.UserID, Constants.Constants.GenericNotificationTopic);
                }
                else
                {
                    r_PushService.UnsubscribeTopics(AppManager.Instance.ConnectedUser.UserID, Constants.Constants.GenericNotificationTopic);
                }
            }

            FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
        }
        #endregion
    }
}
