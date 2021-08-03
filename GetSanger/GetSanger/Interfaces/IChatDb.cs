namespace GetSanger.Interfaces
{
    public interface IChatDb : IUserChatDb, IMessageChatDb
    {
        void DeleteDb(string id = null); //if id = null all Db would be deleted else only the messages and the user with the id will be deleted;
    }
}