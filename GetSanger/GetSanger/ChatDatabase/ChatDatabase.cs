using System.Collections.Generic;
using System.Threading.Tasks;
using GetSanger.Models.chat;
using GetSanger.Services;
using GetSanger.Interfaces;
using Xamarin.Forms;
using SQLite;
using System;
using System.Linq;

namespace GetSanger.ChatDatabase
{
    public class ChatDatabase
    {
        #region Fields
        private static SQLiteAsyncConnection m_Connection;
        #endregion

        #region Instance
        public static readonly AsyncLazy<ChatDatabase> Instance = new AsyncLazy<ChatDatabase>(async () => {
            ChatDatabase db = new ChatDatabase();
            await db.CreateTablesAsnyc();
            await m_Connection.EnableWriteAheadLoggingAsync();
            return db;
        });
        #endregion

        #region Constructor
        public ChatDatabase()
        {
            m_Connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        #endregion

        #region Methods
        public async Task CreateTablesAsnyc()
        {
            await m_Connection.CreateTableAsync<ChatUser>();
            await m_Connection.CreateTableAsync<Message>();
        }

        #region UsersTable
        public async Task<ChatUser> AddUserAsync(string i_UserId, DateTime? i_LastMessage = null)
        {
            ChatUser newUser = new ChatUser
            {
                UserId = i_UserId,
                LastMessage = i_LastMessage != null ? (DateTime)i_LastMessage : DateTime.Now 
            };

            return (await m_Connection.InsertAsync(newUser) == 1) ? newUser : null;
        }

        public async Task<int> DeleteUserAsync(string i_UserId)
        {
            ChatUser toDelete = await m_Connection.Table<ChatUser>().Where(user => user.UserId.Equals(i_UserId)).FirstAsync();
            if(toDelete != null)
            {
                return await m_Connection.DeleteAsync(toDelete);
            }
            else
            {
                return 0;
            }
        }

        public Task<int> UpdateUserAsync(ChatUser i_ToUpdate)
        {
            return m_Connection.UpdateAsync(i_ToUpdate);
        }

        public Task<ChatUser> GetUserAsync(string i_Id)
        {
            return m_Connection.Table<ChatUser>().Where(user => user.UserId.Equals(i_Id)).FirstOrDefaultAsync();
        }

        public Task<List<ChatUser>> GetAllUsersAsync()
        {
            return m_Connection.Table<ChatUser>().ToListAsync();
        }

        #endregion

        #region MessagesTable

        public Task<List<Message>> GetMessagesAsync(string i_UserToChatId)
        {
            string i_MyId = AppManager.Instance.ConnectedUser.UserId;
            return m_Connection.Table<Message>().Where(item => (item.ToId.Equals(i_MyId) && item.FromId.Equals(i_UserToChatId)) 
                                                               || (item.ToId.Equals(i_UserToChatId) && item.FromId.Equals(i_MyId))).ToListAsync();
        }

        public async Task<int> AddMessageAsync(Message i_Message, string i_ChatId) // chat id is most of the time the userTo id
        {
            ChatUser user = null;
            if (i_ChatId != null)
            {
                user = await GetUserAsync(i_ChatId);
                user.LastMessage = i_Message.TimeSent;
            }

            if (user == null)
            {
                user = await AddUserAsync(i_ChatId, i_Message.TimeSent);
            }

            int retVal = await UpdateUserAsync(user);
            if(retVal == 1)
            {
                return await m_Connection.InsertAsync(i_Message);
            }
            else
            {
                throw new ArgumentException("Failed to update user after adding a message");
            }
        }

        public async Task<int> DeleteMessageAsync(Message i_Message)
        {
            int deleted = await m_Connection.DeleteAsync(i_Message);
            if(deleted > 0)
            {
                string id = i_Message.ToId.Equals(AppManager.Instance.ConnectedUser.UserId) ? i_Message.FromId : i_Message.ToId;
                List<Message> messages= await GetMessagesAsync(id);
                if(messages.Count == 0)
                {
                    await DeleteUserAsync(id);
                }
                else
                {
                    ChatUser user = await GetUserAsync(id);
                    user.LastMessage = messages.Last().TimeSent;
                    await UpdateUserAsync(user);
                }
            }

            return deleted;
        }

        public Task UpdateMessageAsync(Message i_Message)
        {
            return m_Connection.UpdateAsync(i_Message);
        }

        #endregion

        #endregion
    }
}
