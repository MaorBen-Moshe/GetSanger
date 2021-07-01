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
            return new SQLiteAsyncConnection(Constants.Constants.ChatDBPath, Constants.Constants.ChatDbFlags);
        }
    }
}