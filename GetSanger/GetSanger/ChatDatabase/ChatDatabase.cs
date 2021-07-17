﻿using System.Collections.Generic;
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
    public class ChatDatabase : IChatDb
    {
        #region Fields
        private static SQLiteAsyncConnection m_Connection;
        #endregion

        #region Instance
        public static readonly AsyncLazy<IChatDb> Instance = new AsyncLazy<IChatDb>(async () => 
                                                                                    {
                                                                                       ChatDatabase db = new ChatDatabase();
                                                                                       await db.CreateTablesAsnyc();
                                                                                       await m_Connection.EnableWriteAheadLoggingAsync();
                                                                                       return db;
                                                                                    });
        #endregion

        #region Constructor
        private ChatDatabase()
        {
            m_Connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        #endregion

        #region Methods
        private async Task CreateTablesAsnyc()
        {
            await m_Connection.CreateTableAsync<ChatUser>();
            await m_Connection.CreateTableAsync<Message>();
        }

        public async void DeleteDb(string id = null)
        {
            if(id == null)
            {
                List<ChatUser> users = await GetAllUsersAsync();
                IUiPush push = AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
                foreach(ChatUser user in users)
                {
                    // send to each device that the user is deleted so he would delete all the messages inside of his db
                    await push.SendToDevice(user.UserId, AuthHelper.GetLoggedInUserId(), null, null, null);
                }

                foreach(TableMapping map in m_Connection.TableMappings)
                {
                    await m_Connection.DeleteAllAsync(map);
                }
            }
            else
            {
                List<Message> messages = await GetAllMessagesAsync();
                await DeleteUserAsync(id);
                foreach(Message message in messages)
                {
                    if(message.FromId.Equals(id) || message.ToId.Equals(id))
                    {
                        await DeleteMessageAsync(message);
                    }
                }
            }
        }

        #region UsersTable
        public async Task<ChatUser> AddUserAsync(string i_UserId, DateTime? i_LastMessage = null, string i_CreatedById = null)
        {
            ChatUser newUser = new ChatUser
            {
                UserId = i_UserId,
                LastMessage = i_LastMessage != null ? (DateTime)i_LastMessage : DateTime.Now,
                UserCreatedById = i_CreatedById ?? AuthHelper.GetLoggedInUserId()
            };

            return (await m_Connection.InsertAsync(newUser) == 1) ? newUser : null;
        }

        public async Task<int> DeleteUserAsync(string i_UserId)
        {
            ChatUser toDelete = await m_Connection.Table<ChatUser>()?.Where(user => user.UserId.Equals(i_UserId) 
                                                                      && user.UserCreatedById.Equals(AuthHelper.GetLoggedInUserId())).FirstAsync();
            int retVal = 0;
            if(toDelete != null)
            {
                retVal = await m_Connection.DeleteAsync(toDelete);
            }

            return retVal;
        }

        public Task<int> UpdateUserAsync(ChatUser i_ToUpdate)
        {
            return m_Connection.UpdateAsync(i_ToUpdate);
        }

        public Task<ChatUser> GetUserAsync(string i_Id, string i_CreatedById = null)
        {
            string currentId = i_CreatedById ?? AuthHelper.GetLoggedInUserId();
            return m_Connection.Table<ChatUser>().Where(user => user.UserId.Equals(i_Id) &&
                                                                user.UserCreatedById != null &&
                                                                user.UserCreatedById.Equals(currentId)).FirstOrDefaultAsync();
        }

        public async Task<List<ChatUser>> GetAllUsersAsync()
        {
            List<ChatUser> users = await m_Connection.Table<ChatUser>().ToListAsync();
            return users?.Where(user => user != null &&
                                        user.UserCreatedById != null &&
                                        user.UserCreatedById.Equals(AuthHelper.GetLoggedInUserId())).ToList();
        }

        #endregion

        #region MessagesTable
        public Task<List<Message>> GetAllMessagesAsync()
        {
            return m_Connection.Table<Message>().ToListAsync();                                     
        }

        public Task<List<Message>> GetMessagesAsync(string i_UserToChatId)
        {
            string i_MyId = AppManager.Instance.ConnectedUser.UserId;
            return m_Connection.Table<Message>()?.Where(item => (item.ToId.Equals(i_MyId) && item.FromId.Equals(i_UserToChatId)) ||
                                                                (item.ToId.Equals(i_UserToChatId) && item.FromId.Equals(i_MyId))).ToListAsync();
        }

        public async Task<int> AddMessageAsync(Message i_Message, string i_ChatId, string i_CreatedById = null) // chat id is most of the time the userTo id
        {
            ChatUser user = null;
            int retVal = 0;
            if (i_ChatId != null)
            {
                user = await GetUserAsync(i_ChatId, i_CreatedById);
                if (user == null)
                {
                    user = await AddUserAsync(i_ChatId, i_Message.TimeSent, i_CreatedById ?? AuthHelper.GetLoggedInUserId());
                    if (user != null)
                    {
                        retVal = 1;
                    }
                }
                else
                {
                    user.LastMessage = i_Message.TimeSent;
                    retVal = await UpdateUserAsync(user);
                }
            }
            else
            {
                throw new ArgumentNullException("You must provide chat id!");
            }

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
