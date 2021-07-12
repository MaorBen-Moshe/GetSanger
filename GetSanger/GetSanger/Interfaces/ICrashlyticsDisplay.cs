using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Interfaces
{
    public interface ICrashlyticsDisplay : ICrashlytics
    {
        void LogPageEntrance(string i_PageName);
    }
}
