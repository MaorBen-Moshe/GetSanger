using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Interfaces
{
    public interface ICrashlytics
    {
        void SetCustomKeys(Dictionary<string, object> i_Dictionary);
        public void SetCustomKey(string key, object value);
        void AddCustomLogMessage(string i_Message);
        void SetUserId(string i_UserId);
        void RecordException(Exception i_Exception);
    }
}
