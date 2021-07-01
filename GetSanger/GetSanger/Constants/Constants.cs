using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Constants
{
    public static class Constants
    {
        public static string MapsApiKey { get; } = "AIzaSyCtfUx4NamOLsRFRZ0cMrh-cG-C0K_JTVA";

        public static string GenericNotificationTopic { get; } = "Generic";

        public static string LocationMessage = "Location";

        public static string ActivatedLocationMessage = "ActivatedLocation";

        public static string ChatDatabaseName = "Chat.db3";

        public static SQLite.SQLiteOpenFlags ChatDbFlags = 
                                                            SQLiteOpenFlags.ReadWrite | 
                                                            SQLiteOpenFlags.Create | 
                                                            SQLiteOpenFlags.SharedCache;
    }
}
