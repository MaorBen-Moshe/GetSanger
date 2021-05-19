using GetSanger.Constants;
using GetSanger.Models.chat;
using GetSanger.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels.chat
{
    public class ChatListViewModel : BaseViewModel
    {
        #region Fields
        #endregion

        #region Properties
        public ObservableCollection<ChatUser> Users { get; set; }
        #endregion

        #region Commands
        public ICommand UserSelectedCommand { get; set; }
        #endregion

        #region Constructor
        public ChatListViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods
        public async override void Appearing()
        {
            ChatDatabase.ChatDatabase database = AppManager.Instance.Services.GetService(typeof(ChatDatabase.ChatDatabase)) as ChatDatabase.ChatDatabase;
            List<ChatUser> users = await database.GetChatUsers();
            Users = new ObservableCollection<ChatUser>(users.OrderBy(user => user.LastMessage));
        }

        private void setCommands()
        {
            UserSelectedCommand = new Command(userSelected);
        }

        private async void userSelected(object i_Param)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.ChatView + $"?userTo={(i_Param as ChatUser).User}");
        }
        #endregion
    }
}
