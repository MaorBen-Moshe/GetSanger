using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Constants
{
    public static class SqliteConstants
    {
        public const string DatabaseFilename = "ChatMessages.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                string folder = string.Empty, databasePath;
                if (Device.RuntimePlatform.Equals(Device.iOS))
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    folder = folder + ".." + "Library";
                }
                else if (Device.RuntimePlatform.Equals(Device.Android))
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
                    
                databasePath = folder + '/' + DatabaseFilename;
                return databasePath;

                //var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //var to = basePath + DatabaseFilename;
                //return to;
            }
        }
    }
}
