using System.Collections.Generic;
using System.Threading.Tasks;
using GetSanger.Models.chat;
using System.Linq;
using GetSanger.Services;
using Xamarin.Essentials;
using GetSanger.Interfaces;
using Xamarin.Forms;
using SQLite;
using GetSanger.Models;
using System;

namespace GetSanger.ChatDatabase
{
    public class ChatDatabase : Service
    {
        #region Fields
        private SQLiteAsyncConnection m_Connection;
        private bool m_IsUsersCreated;
        private bool m_IsMessagesCreated;
        #endregion

        #region Constructor
        public ChatDatabase()
        {
            m_IsUsersCreated = false;
            m_IsMessagesCreated = false;
        }
        #endregion

        #region Methods
        public async override void SetDependencies()
        {
            m_Connection ??= DependencyService.Get<ISQLiteDb>().GetConnection();
            if (!m_IsUsersCreated)
            {
                await m_Connection.CreateTableAsync<ChatUser>();
                m_IsUsersCreated = true;
            }
            if (!m_IsMessagesCreated)
            {
                await m_Connection.CreateTableAsync<Message>();
                m_IsMessagesCreated = true;
            }
        }

        public Task<int> AddUserAsync(string i_UserId, DateTime? i_LastMessage = null)
        {
            SetDependencies();
            ChatUser newUser = new ChatUser
            {
                UserId = i_UserId,
                LastMessage = i_LastMessage != null ? (DateTime)i_LastMessage : DateTime.Now 
            };

            return m_Connection.InsertAsync(newUser);
        }

        public async Task<int> DeleteUserAsync(string i_UserId)
        {
            SetDependencies();
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

        public Task<ChatUser> GetUserAsync(string i_Id)
        {
            return m_Connection.Table<ChatUser>().Where(user => user.UserId.Equals(i_Id)).FirstAsync();
        }

        public Task<List<ChatUser>> GetAllUsersAsync()
        {
            SetDependencies();
            return m_Connection.Table<ChatUser>().ToListAsync();
        }

        public Task<List<Message>> GetMessagesAsync(string i_UserToChatId)
        {
            SetDependencies();
            string i_MyId = AppManager.Instance.ConnectedUser.UserId;
            return m_Connection.Table<Message>().Where(item => (item.ToId.Equals(i_MyId) && item.FromId.Equals(i_UserToChatId)) 
                                                               || (item.ToId.Equals(i_UserToChatId) && item.FromId.Equals(i_MyId))).ToListAsync();
        }

        public async Task<int> AddMessageAsync(Message i_Message, string i_ChatId) // chat id is most of the time the userTo id
        {
            SetDependencies();
            ChatUser user = null;
            if (i_ChatId != null)
            {
                user = await GetUserAsync(i_ChatId);
            }
            
            if(user == null)
            {
                await AddUserAsync(i_ChatId, i_Message.TimeSent);
            }

            return await m_Connection.InsertAsync(i_Message);
        }

        public async Task<int> DeleteMessageAsync(Message i_Message)
        {
            SetDependencies();
            int deleted = await m_Connection.DeleteAsync(i_Message);
            if(deleted > 0)
            {
                string id = i_Message.ToId.Equals(AppManager.Instance.ConnectedUser.UserId) ? i_Message.FromId : i_Message.ToId;
                List<Message> messages= await GetMessagesAsync(id);
                if(messages.Count == 0)
                {
                    await DeleteUserAsync(id);
                }
            }

            return deleted;
        }

        public Task UpdateMessageAsync(Message i_Message)
        {
            SetDependencies();
            return m_Connection.UpdateAsync(i_Message);
        }
        #endregion
    }
}
