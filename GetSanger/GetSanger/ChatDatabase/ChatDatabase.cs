using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetSanger.Models.chat;
using System.IO;
using PCLStorage;
using Xamarin.Forms;
using System.Text.Json;
using System.Linq;

namespace GetSanger.ChatDatabase
{
    public class ChatDatabase
    {
        #region Fields
        private const string k_DatabaseFilename = "ChatMessages.db3";
        #endregion

        #region Properties
        //public static string DatabasePath
        //{
        //    get
        //    {
        //        string folder = string.Empty, databasePath;
        //        if (Device.RuntimePlatform.Equals(Device.iOS))
        //        {
        //            folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //            folder = folder + ".." + "Library";
        //        }
        //        else if (Device.RuntimePlatform.Equals(Device.Android))
        //        {
        //            folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //        }

        //        databasePath = Path.Combine(folder, k_DatabaseFilename);
        //        return databasePath;
        //    }
        //}
        #endregion

        #region Constructor
        public ChatDatabase()
        {
        }
        #endregion

        #region Methods

        public async Task<List<Message>> GetItemsAsync(string i_ToID)
        {
            List<Message> messages = await getDB(i_ToID);
            return messages;
        }

        public async Task SaveItemAsync(Message i_Item, string i_ToID)
        {
            List<Message> messages = await getDB(i_ToID);
            messages.Insert(0, i_Item);
            await setDB(messages, i_ToID);
        }

        public async Task DeleteItemAsync(Message i_Item, string i_ToID)
        {
            List<Message> messages = await getDB(i_ToID);
            messages = messages.Where(msg => msg.Equals(i_Item) == false).ToList();
            await setDB(messages, i_ToID);
        }

        private async Task<IFolder> createMessagesFolder()
        {
            IFolder root = FileSystem.Current.LocalStorage;
            return await root.CreateFolderAsync("Messages", CreationCollisionOption.OpenIfExists);
        }

        private async Task<IFile> openMessagesDB(string i_ToID)
        {
            IFolder current = await createMessagesFolder();
            ExistenceCheckResult result = await current.CheckExistsAsync(i_ToID);
            IFile MessagesFile;
            if (result == ExistenceCheckResult.NotFound)
            {
                MessagesFile = await current.CreateFileAsync(i_ToID, CreationCollisionOption.OpenIfExists);
            }
            else
            {
                MessagesFile = await current.GetFileAsync(i_ToID);
            }

            return MessagesFile;
        }

        private async Task<List<Message>> getDB(string i_ToID)
        {
            IFile current = await openMessagesDB(i_ToID);
            string json = await current.ReadAllTextAsync();
            List<Message> messages;
            if (string.IsNullOrWhiteSpace(json))
            {
                messages = new List<Message>();
            }
            else
            {
                messages = JsonSerializer.Deserialize<List<Message>>(json);
            }

            return messages;
        }

        private async Task setDB(List<Message> i_Data, string i_ToID)
        {
            IFile current = await openMessagesDB(i_ToID);
            string json = JsonSerializer.Serialize(i_Data);
            await current.WriteAllTextAsync(json);
        }
        #endregion
    }
}
