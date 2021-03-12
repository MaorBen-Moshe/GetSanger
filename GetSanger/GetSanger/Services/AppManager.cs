using System;

namespace GetSanger.Services
{
    public sealed class AppManager
    {
        public event Action Refresh_Event;

        public static AppManager Instance { get => Singleton<AppManager>.Instance; }

        private AppManager()
        {

        }
    }
}
