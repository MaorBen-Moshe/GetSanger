using GetSanger.Models.chat;
using System;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IUiPush : IPushService
    {
        Task SendToDevice<T>(string i_UserId, T i_Data, Type i_DataType, string i_Title = "", string i_Message = null) where T : class ;

        Task RegisterTopics(string i_UserId, params string[] i_Topics);

        Task UnsubscribeTopics(string i_UserId, params string[] i_Topics);

        Task<bool> IsRegistrationTokenChanged();

        Task UnsubscribeUser(string i_UserId);

        Task SubscribeUser(string i_UserId);

        void SendChatMessage(Message i_Message);
    }
}
