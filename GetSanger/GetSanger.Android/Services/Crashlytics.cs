using System.Collections.Generic;
using Firebase.Crashlytics;
using GetSanger.Droid.Services;
using GetSanger.Interfaces;
using Java.Lang;
using Xamarin.Forms;
using static Firebase.Crashlytics.FirebaseCrashlytics;
using Exception = System.Exception;

[assembly: Dependency(typeof(Crashlytics))]

namespace GetSanger.Droid.Services
{
    public class Crashlytics : ICrashlytics
    {
        public void SetCustomKeys(Dictionary<string, object> i_Dictionary)
        {
            CustomKeysAndValues.Builder builder = new CustomKeysAndValues.Builder();

            foreach (string key in i_Dictionary.Keys)
            {
                object value = i_Dictionary[key];

                if (value is string strValue)
                {
                    builder.PutString(key, strValue);
                }
                else if (value is bool boolValue)
                {
                    builder.PutBoolean(key, boolValue);
                }
                else if (value is int intValue)
                {
                    builder.PutInt(key, intValue);
                }
                else if (value is long longValue)
                {
                    builder.PutLong(key, longValue);
                }
                else if (value is float floatValue)
                {
                    builder.PutFloat(key, floatValue);
                }
                else if (value is double doubleValue)
                {
                    builder.PutDouble(key, doubleValue);
                }
            }

            CustomKeysAndValues customKeysAndValues = builder.Build();
            Instance.SetCustomKeys(customKeysAndValues);
        }

        public void SetCustomKey(string key, object value) 
        {
            if(key != null)
            {
                if (value is string strValue)
                {
                    Instance.SetCustomKey(key, strValue);
            }
                else if (value is bool boolValue)
                {
                    Instance.SetCustomKey(key, boolValue);
                }
                else if (value is int intValue)
                {
                    Instance.SetCustomKey(key, intValue);
                }
                else if (value is long longValue)
                {
                    Instance.SetCustomKey(key, longValue);
                }
                else if (value is float floatValue)
                {
                    Instance.SetCustomKey(key, floatValue);
                }
                else if (value is double doubleValue)
                {
                    Instance.SetCustomKey(key, doubleValue);
                }
            }
        }

        public void AddCustomLogMessage(string i_Message)
        {
            Instance.Log(i_Message);
        }

        public void SetUserId(string i_UserId)
        {
            Instance.SetUserId(i_UserId);
        }

        public void RecordException(Exception i_Exception)
        {
            Instance.RecordException(Throwable.FromException(i_Exception));
        }
    }
}