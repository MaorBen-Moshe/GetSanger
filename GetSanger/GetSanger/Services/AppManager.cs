using GetSanger.Models;
using GetSanger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetSanger.Services
{
    public enum AppMode { Client, Sanger };

    public sealed class AppManager
    {
        public event Action Refresh_Event;

        public static AppManager Instance { get => Singleton<AppManager>.Instance; }

        public AppMode CurrentMode { get; set; }

        public User ConnectedUser { get; set; }

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

        private void setAppManager()
        {
            Services = new AppServices();
            Services.Add(typeof(LocationService), new LocationService());
            Services.Add(typeof(DialServices), new DialServices());
            Services.Add(typeof(LoginServices), new LoginServices());
            Services.Add(typeof(NavigationService), new NavigationService());
            Services.Add(typeof(PageServices), new PageServices());
            Services.Add(typeof(PushServices), new PushServices());
        }

        public void RefreshAppManager()
        {
            setAppManager();
            Services.SetDependencies();
            Refresh_Event?.Invoke();
        }
    }
}
