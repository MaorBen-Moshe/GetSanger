using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetSanger.Models.chat;
using System.IO;
using PCLStorage;
using Xamarin.Forms;
using System.Text.Json;
using System.Linq;
using GetSanger.Services;
using Xamarin.Essentials;

namespace GetSanger.ChatDatabase
{
    public class ChatDatabase : Service
    {
        #region Fields
        private const string k_DatabaseFilename = "ChatMessages.db3";
        #endregion

        #region Constructor
        public ChatDatabase()
        {
        }
        #endregion

        #region Methods
        public override void SetDependencies()
        {
        }

        public async Task<List<Message>> GetItemsAsync(string i_ToID)
        {
            List<Message> messages = await getDB(i_ToID);
            return messages;
        }

        public async Task SaveItemAsync(Message i_Item, string i_ToID)
        {
            List<Message> messages = await getDB(i_ToID);
            messages.Insert(0, i_Item);
            setDB(messages, i_ToID);
        }

        public async Task UpdateSentItemAsync(Message i_Item, string i_ToID)
        {
            List<Message> messages = await getDB(i_ToID);
            foreach(Message msg in messages)
            {
                if (msg.Equals(i_Item))
                {
                    msg.MessageSent = true;
                    break;
                }
            }

            setDB(messages, i_ToID);
        }

        public async Task DeleteItemAsync(Message i_Item, string i_ToID)
        {
            List<Message> messages = await getDB(i_ToID);
            messages = messages.Where(msg => msg.Equals(i_Item) == false).ToList();
            setDB(messages, i_ToID);
        }

        private async Task<IFolder> createMessagesFolder()
        {
            IFolder root = PCLStorage.FileSystem.Current.LocalStorage;
            return await root.CreateFolderAsync("Messages", CreationCollisionOption.OpenIfExists);
        }

        private async Task<IFile> openMessagesDB(string i_ToID)
        {
            IFolder current = await createMessagesFolder();
            return await current.CreateFileAsync(i_ToID, CreationCollisionOption.OpenIfExists); ;
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

        private void setDB(List<Message> i_Data, string i_ToID)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                IFile current = await openMessagesDB(i_ToID);
                string json = JsonSerializer.Serialize(i_Data);
                await current.WriteAllTextAsync(json);
            });

        }
        #endregion
    }
}
