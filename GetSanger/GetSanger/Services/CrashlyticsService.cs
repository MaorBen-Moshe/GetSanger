using System;
using System.Collections.Generic;
using GetSanger.Interfaces;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class CrashlyticsService : Service
    {
        private static readonly ICrashlytics sr_Crashlytics = DependencyService.Get<ICrashlytics>();

        /// <summary>
        /// Custom keys help you get the specific state of your app leading up to a crash.
        /// You can associate arbitrary key/value pairs with your crash reports, then use the custom keys to search and filter crash reports in the Firebase console. 
        /// </summary>
        /// <param name="i_Dictionary">A dictionary containing the key/value pairs to set.</param>
        public void SetCustomKeys(Dictionary<string, object> i_Dictionary)
        {
            sr_Crashlytics.SetCustomKeys(i_Dictionary);
        }

        public void SetCustomKey(string key, object value)
        {
            sr_Crashlytics.SetCustomKey(key, value);
        }

        /// <summary>
        /// To give yourself more context for the events leading up to a crash, you can add custom Crashlytics logs to your app.
        /// Crashlytics associates the logs with your crash data and displays them in the Crashlytics page of the Firebase console, under the Logs tab.
        /// </summary>
        /// <param name="i_Message">The message to log.</param>
        public void AddCustomLogMessage(string i_Message)
        {
            if(i_Message != null)
            {
                sr_Crashlytics.AddCustomLogMessage(i_Message);
            }
        }

        /// <summary>
        /// To diagnose an issue, it’s often helpful to know which of your users experienced a given crash.
        /// Crashlytics includes a way to anonymously identify users in your crash reports.
        /// </summary>
        /// <param name="i_UserId">The unique id of the user.</param>
        public void SetUserId(string i_UserId = "0") // if userid == 0  its mean the crash occurred in auth shell
        {
            sr_Crashlytics.SetUserId(i_UserId);
        }

        /// <summary>
        /// In addition to automatically reporting your app’s crashes, Crashlytics lets you record non-fatal exceptions and sends them to you the next time your app launches.
        /// </summary>
        /// <param name="i_Exception">The exception to record.</param>
        public void RecordException(Exception i_Exception)
        {
            sr_Crashlytics.RecordException(i_Exception);
        }

        public override void SetDependencies()
        {
        }
    }
}