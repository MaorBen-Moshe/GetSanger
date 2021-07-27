using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetSanger.Constants
{
    public static class Constants
    {
        public static string MapsApiKey { get; } = "AIzaSyCtfUx4NamOLsRFRZ0cMrh-cG-C0K_JTVA";

        public static string GenericNotificationTopic { get; } = "Generic";

        public static string LocationMessage { get; }  = "Location";

        public static string EndActivity { get; } = "ActivatedLocation";

        public static string SangerNotesSent { get; } = "SangerNotesSent";

        public static string StartProperty = "Start";

        public static string ChatDatabaseName { get; } = "Chat.db3";

        public static SQLite.SQLiteOpenFlags ChatDbFlags { get; }  = 
                                                            SQLiteOpenFlags.ReadWrite | 
                                                            SQLiteOpenFlags.Create | 
                                                            SQLiteOpenFlags.SharedCache;
        public static string ChatDBPath
        {
            get
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(documentsPath, ChatDatabaseName);
            }
        }
    }
}
