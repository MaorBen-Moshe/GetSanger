using System;
using System.IO;
using GetSanger.Droid.Persistence;
using GetSanger.Interfaces;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDb))]
namespace GetSanger.Droid.Persistence
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, Constants.Constants.ChatDatabaseName);
            return new SQLiteAsyncConnection(path, Constants.Constants.ChatDbFlags);
        }
    }
}