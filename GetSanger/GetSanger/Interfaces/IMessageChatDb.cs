using GetSanger.Models.chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IMessageChatDb
    {
        public Task<List<Message>> GetAllMessagesAsync();

        Task<List<Message>> GetMessagesAsync(string i_UserToChatId);

        Task<int> AddMessageAsync(Message i_Message, string i_ChatId, string i_CreatedById = null);

        Task<int> DeleteMessageAsync(Message i_Message);

        Task UpdateMessageAsync(Message i_Message);
    }
}