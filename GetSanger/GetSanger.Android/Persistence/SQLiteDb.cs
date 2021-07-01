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
            return new SQLiteAsyncConnection(Constants.Constants.ChatDBPath, Constants.Constants.ChatDbFlags);
        }
    }
}