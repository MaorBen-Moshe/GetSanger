using GetSanger.Models.chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IUserChatDb
    {
        Task<ChatUser> AddUserAsync(string i_UserId, DateTime? i_LastMessage = null, string i_CreatedById = null);

        Task<int> DeleteUserAsync(string i_UserId);

        Task<int> UpdateUserAsync(ChatUser i_ToUpdate);

        Task<ChatUser> GetUserAsync(string i_Id, string i_CreatedById = null);

        Task<List<ChatUser>> GetAllUsersAsync();
    }
}
