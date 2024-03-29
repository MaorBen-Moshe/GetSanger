﻿using GetSanger.Models;
using GetSanger.ViewModels;
using System;
using GetSanger.AppShell;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace GetSanger.Services
{
    public enum eAppMode { Client, Sanger };

    public sealed class AppManager
    {
        private UserShell m_UserShell;
        private SangerShell m_SangerShell;
        private User m_User;
        private readonly object m_UserLock = new object();

        public event Action Refresh_Event;

        public static AppManager Instance { get => Singleton<AppManager>.Instance; }

        public eAppMode CurrentMode { get; set; }

        public User ConnectedUser
        {
            get
            {
                lock (m_UserLock)
                {
                    return m_User;
                }
            }
            set
            {
                lock (m_UserLock)
                {
                    m_User = value;
                    if (m_User?.LastUserMode != null)
                    {
                        CurrentMode = (eAppMode)m_User.LastUserMode;
                    }
                }
            }
        }

        public SignUpPageViewModel SignUpVM { get; set; }

        public AppServices Services { get; set; }

        private AppManager()
        {
            setAppManager();
        }

        public void RefreshAppManager()
        {
            setAppManager();
            Services.SetDependencies();
            Refresh_Event?.Invoke();
        }

        public Shell GetCurrentShell(eAppMode? i_Mode = null)
        {
            eAppMode mode = i_Mode != null ? (eAppMode)i_Mode : CurrentMode;
            updateMode(i_Mode);
            Shell toRet = mode switch
            {
                eAppMode.Client => m_UserShell,
                eAppMode.Sanger => m_SangerShell,
                _ => null,
            };

            return toRet;
        }

        private async void updateMode(eAppMode? i_Mode)
        {
            if(i_Mode != null)
            {
                eAppMode mode = (eAppMode)i_Mode;
                CurrentMode = mode;
                ConnectedUser.LastUserMode = mode;
                await Task.Run(async () => await FireStoreHelper.UpdateUser(ConnectedUser));
            }
        }

        private void setAppManager()
        {
            Services = new AppServices();
            Services.Add(typeof(LocationService), new LocationService());
            Services.Add(typeof(DialServices), new DialServices());
            Services.Add(typeof(LoginServices), new LoginServices());
            Services.Add(typeof(NavigationService), new NavigationService());
            Services.Add(typeof(PageServices), new PageServices());
            Services.Add(typeof(PushServices), new PushServices());
            Services.Add(typeof(StorageHelper), new StorageHelper());
            Services.Add(typeof(PhotoDisplayService), new PhotoDisplayService());
            Services.Add(typeof(SocialAdapterService), new SocialAdapterService());
            Services.Add(typeof(LoadingService), new LoadingService());
            Services.Add(typeof(RunTasksService), new RunTasksService());
            Services.Add(typeof(CrashlyticsService), new CrashlyticsService());

            m_UserShell = new UserShell();
            m_SangerShell = new SangerShell();
        }
    }
}