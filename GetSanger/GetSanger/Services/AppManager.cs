using GetSanger.Models;
using GetSanger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using GetSanger.AppShell;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public enum AppMode { Client, Sanger };

    public sealed class AppManager
    {
        private UserShell m_UserShell;
        private SangerShell m_SangerShell;
        private User m_User;
        private object m_UserLock = new object();

        public event Action Refresh_Event;

        public static AppManager Instance { get => Singleton<AppManager>.Instance; }

        public AppMode CurrentMode { get; set; }

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
                        CurrentMode = (AppMode)m_User.LastUserMode;
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

        public IList<string> GetListOfEnumNames(Type i_EnumType)
        {
            return (from name in i_EnumType.GetEnumNames() select name).ToList();
        }

        public void RefreshAppManager()
        {
            setAppManager();
            Services.SetDependencies();
            Refresh_Event?.Invoke();
        }

        public Shell GetCurrentShell(AppMode? i_Mode = null)
        {
            AppMode mode = i_Mode != null ? (AppMode)i_Mode : CurrentMode;
            updateMode(i_Mode);
            Shell toRet = mode switch
            {
                AppMode.Client => m_UserShell,
                AppMode.Sanger => m_SangerShell,
                _ => null,
            };

            return toRet;
        }

        private async void updateMode(AppMode? i_Mode)
        {
            if(i_Mode != null)
            {
                AppMode mode = (AppMode)i_Mode;
                CurrentMode = mode;
                ConnectedUser.LastUserMode = mode;
                await FireStoreHelper.UpdateUser(ConnectedUser);
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
            Services.Add(typeof(ChatDatabase.ChatDatabase), new ChatDatabase.ChatDatabase());
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
