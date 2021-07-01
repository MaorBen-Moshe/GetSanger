using System;
using System.IO;
using GetSanger.Interfaces;
using GetSanger.iOS.Persistence;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDb))]
namespace GetSanger.iOS.Persistence
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(documentsPath, Constants.Constants.ChatDatabaseName);
            return new SQLiteAsyncConnection(path, Constants.Constants.ChatDbFlags);
        }
    }
}