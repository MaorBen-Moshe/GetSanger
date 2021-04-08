using GetSanger.Models;
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

        private AppManager()
        {

        }

        public IList<string> GetListOfEnum(Type i_EnumType)
        {
            return i_EnumType.GetEnumNames().ToList();
        }
    }
}
