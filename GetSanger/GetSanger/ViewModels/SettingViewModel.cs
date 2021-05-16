﻿using GetSanger.AppShell;
using GetSanger.Models;
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

        public ICommand DeleteAccountCommand { get; set; }

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
            AppManager.Instance.ConnectedUser = new User
            {
                IsGenericNotifications = true,
                Categories = new ObservableCollection<Category> { Category.Cleaning, Category.Electrician, Category.Delivery }
            };

            IsGenericNotificatons = AppManager.Instance.ConnectedUser.IsGenericNotifications;
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
        }

        private void setCommands()
        {
            ToggledCommand = new Command(toggled);
            DeleteAccountCommand = new Command(deleteAccount);
        }

        private async void toggled(object i_Param)
        {
            if (i_Param is CategoryCell)
            {
                CategoryCell current = i_Param as CategoryCell;
                if (current.Checked)
                {
                    AppManager.Instance.ConnectedUser.Categories.Add(current.Category);
                    await RunTaskWhileLoading(r_PushService.RegisterTopics(AppManager.Instance.ConnectedUser.UserId, ((int)current.Category).ToString()));
                }
                else
                {
                    AppManager.Instance.ConnectedUser.Categories.Remove(current.Category);
                    await RunTaskWhileLoading(r_PushService.UnsubscribeTopics(AppManager.Instance.ConnectedUser.UserId, ((int)current.Category).ToString()));
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

            await RunTaskWhileLoading(FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser));
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
