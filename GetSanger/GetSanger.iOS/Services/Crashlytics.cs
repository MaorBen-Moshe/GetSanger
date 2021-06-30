using System;
using System.Collections.Generic;
using Firebase.Crashlytics;
using Foundation;
using GetSanger.Interfaces;
using Xamarin.Forms;
using static Firebase.Crashlytics.Crashlytics;
using Crashlytics = GetSanger.iOS.Services.Crashlytics;

[assembly: Dependency(typeof(Crashlytics))]

namespace GetSanger.iOS.Services
{
    public class Crashlytics : ICrashlytics
    {
        public void SetCustomKeys(Dictionary<string, object> i_Dictionary)
        {
            foreach (string key in i_Dictionary.Keys)
            {
                object value = i_Dictionary[key];
                SetCustomKey(key, value);
            }
        }

        public void AddCustomLogMessage(string i_Message)
        {
            SharedInstance.Log(i_Message);
        }

        public void SetUserId(string i_UserId)
        {
            SharedInstance.SetUserId(i_UserId);
        }

        public void RecordException(Exception i_Exception)
        {
            SharedInstance.RecordExceptionModel(ExceptionModel.Create(i_Exception.Message, i_Exception.StackTrace));
        }

        public void SetCustomKey(string key, object value)
        {
            if(key != null)
            {
                if (value is string strValue)
                {
                    SharedInstance.SetCustomValue(NSObject.FromObject(strValue), key);
                }
                else if (value is bool boolValue)
                {
                    SharedInstance.SetCustomValue(NSObject.FromObject(boolValue), key);
                }
                else if (value is int intValue)
                {
                    SharedInstance.SetCustomValue(NSObject.FromObject(intValue), key);
                }
                else if (value is long longValue)
                {
                    SharedInstance.SetCustomValue(NSObject.FromObject(longValue), key);
                }
                else if (value is float floatValue)
                {
                    SharedInstance.SetCustomValue(NSObject.FromObject(floatValue), key);
                }
                else if (value is double doubleValue)
                {
                    SharedInstance.SetCustomValue(NSObject.FromObject(doubleValue), key);
                }
            }
        }
    }
}