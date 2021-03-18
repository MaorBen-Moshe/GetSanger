using GetSanger.Models;
using System;

namespace GetSanger.Services
{
    public enum AppMode { User, Sanger };

    public sealed class AppManager
    {
        public event Action Refresh_Event;

        public static AppManager Instance { get => Singleton<AppManager>.Instance; }

        public AppMode CurrentMode { get; set; }

        public User ConnectedUser { get; set; }

        private AppManager()
        {

        }
    }
}
