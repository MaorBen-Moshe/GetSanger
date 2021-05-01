using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetSanger.Models.chat;
using System.IO;

namespace GetSanger.ChatDatabase
{
    public class ChatDatabase
    {
        #region Fields
        static SQLiteAsyncConnection m_Database;
        #endregion

        #region Properties
        public long DBCount
        {
            get
            {
                return new FileInfo(m_Database.DatabasePath).Length;
            }
        }

        #endregion

        #region Constructor
        public ChatDatabase(string i_ToId)
        {
            string dbPath = Path.Combine(Constants.SqliteConstants.DatabasePath, ("/" + i_ToId));
            m_Database = new SQLiteAsyncConnection(dbPath, Constants.SqliteConstants.Flags);
        }
        #endregion

        #region Methods
        public static AsyncLazy<ChatDatabase> CreateOrGetDataBase(string i_UserIdDB)
        {
            return new AsyncLazy<ChatDatabase>(async () =>
            {
                var instance = new ChatDatabase(i_UserIdDB);
                CreateTableResult result = await m_Database.CreateTableAsync<Message>();
                return instance;
            });
        }

        public Task<List<Message>> GetItemsAsync()
        {
            return m_Database.Table<Message>().ToListAsync();
        }

        public Task<int> SaveItemAsync(Message i_Item)
        {
            return m_Database.InsertAsync(i_Item);
        }

        public Task<int> DeleteItemAsync(Message i_Item)
        {
            return m_Database.DeleteAsync(i_Item);
        }

        #endregion
    }
}
