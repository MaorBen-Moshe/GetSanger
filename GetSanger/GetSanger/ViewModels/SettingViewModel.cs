using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views;
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
        private double m_DistanceLimit;
        private bool m_IsSangerMode;
        private bool m_InfinityChecked;
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

        public double DistanceLimit
        {
            get => m_DistanceLimit;
            set => SetStructProperty(ref m_DistanceLimit, value);
        }

        public bool IsSangerMode
        {
            get => m_IsSangerMode;
            set => SetStructProperty(ref m_IsSangerMode, value);
        }

        public bool InfinityChecked
        {
            get => m_InfinityChecked;
            set => SetStructProperty(ref m_InfinityChecked, value);
        }
        #endregion

        #region Commands
        public ICommand ToggledCommand { get; set; }

        public ICommand BackButtonCommand { get; set; }

        public ICommand DistanceChangedCommand { get; set; }

        public ICommand InfinityCheckedCommand { get; set; }

        #endregion

        #region Constructor
        public SettingViewModel()
        {
            SetCommands();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(SettingViewModel));
            CategoriesItems = new ObservableCollection<CategoryCell>(
            (from
                category in typeof(eCategory).GetListOfEnumNames()
             where
                !category.Equals(eCategory.All.ToString())
             select
                new CategoryCell
                {
                    Category = (eCategory)Enum.Parse(typeof(eCategory), category),
                    Checked = AppManager.Instance.ConnectedUser.Categories.Contains((eCategory)Enum.Parse(typeof(eCategory), category))
                }
            ).ToList());
            IsGenericNotificatons = AppManager.Instance.ConnectedUser.IsGenericNotifications;
            IsSangerMode = AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger);
            DistanceLimit = 10;
            InfinityChecked = false;
        }

        public override void Disappearing()
        {
            if(m_NewCategoriesSubscribed != null && m_NewCategoriesUnsubscribed != null)
            {
                BackButtonCommand.Execute(null);
            }
        }

        protected override void SetCommands()
        {
            ToggledCommand = new Command(toggled);
            BackButtonCommand = new Command(backButtonBehavior);
            DistanceChangedCommand = new Command(distanceChanged);
            InfinityCheckedCommand = new Command(infinityChecked);
        }

        private async void backButtonBehavior()
        {
            try
            {
                bool isChanged = false;
                sr_LoadingService.ShowLoadingPage(new LoadingPage("Saving..."));
                if (m_NewCategoriesSubscribed?.Count > 0)
                {
                    isChanged = true;
                    await sr_PushService.RegisterTopics(AppManager.Instance.ConnectedUser.UserId, m_NewCategoriesSubscribed.ToArray());
                }
                if (m_NewCategoriesUnsubscribed?.Count > 0)
                {
                    isChanged = true;
                    await sr_PushService.UnsubscribeTopics(AppManager.Instance.ConnectedUser.UserId, m_NewCategoriesUnsubscribed.ToArray());
                }
                if (IsGenericNotificatons != AppManager.Instance.ConnectedUser.IsGenericNotifications)
                {
                    isChanged = true;
                    AppManager.Instance.ConnectedUser.IsGenericNotifications = IsGenericNotificatons;
                    genericUpdateHelper();
                }

                if (isChanged)
                {
                    await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                }

                m_NewCategoriesSubscribed = m_NewCategoriesUnsubscribed = null;
                sr_LoadingService.HideLoadingPage();
                await GoBack();
            }
            catch(Exception e)
            {
                sr_LoadingService.HideLoadingPage();
                await e.LogAndDisplayError($"{nameof(SettingViewModel)}:backButtonBehavior", "Error", e.Message);
            }
        }

        private async void distanceChanged()
        {
            if(AppManager.Instance.ConnectedUser.DistanceLimit != DistanceLimit)
            {
                AppManager.Instance.ConnectedUser.DistanceLimit = DistanceLimit;
                await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
            }
        }

        private async void infinityChecked()
        {
            if (InfinityChecked)
            {
                AppManager.Instance.ConnectedUser.DistanceLimit = -1;
                await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
            }
        }

        private async void toggled(object i_Param)
        {
            try
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
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(SettingViewModel)}:toggled", "Error", e.Message);
            }
        }

        private async void genericUpdateHelper()
        {
            if (IsGenericNotificatons)
            {
                await sr_PushService.RegisterTopics(AppManager.Instance.ConnectedUser.UserId, Constants.Constants.GenericNotificationTopic);
            }
            else
            {
                await sr_PushService.UnsubscribeTopics(AppManager.Instance.ConnectedUser.UserId, Constants.Constants.GenericNotificationTopic);
            }
        }

        #endregion
    }
}