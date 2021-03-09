using System;

namespace GetSanger.Services
{
    public class AppManager
    {
        public event Action Refresh_Event;

        public static AppManager Instance { get => Singleton<AppManager>.Instance; }
    }
}
