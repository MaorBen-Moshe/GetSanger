using GetSanger.Constants;
using GetSanger.Models;
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
        private bool m_IsRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<ChatUser> Users { get; set; }

        public bool IsRefreshing
        {
            get => m_IsRefreshing;
            set => SetStructProperty(ref m_IsRefreshing, value);
        }
        #endregion

        #region Commands
        public ICommand UserSelectedCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        #endregion

        #region Constructor
        public ChatListViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            setUsers();
        }

        private void setCommands()
        {
            UserSelectedCommand = new Command(userSelected);
            RefreshCommand = new Command(refresh);
        }

        private void refresh()
        {
            setUsers();
            IsRefreshing = false;
        }

        private async void userSelected(object i_Param)
        {
            string json = ObjectJsonSerializer.SerializeForPage(i_Param as User);
            await r_NavigationService.NavigateTo(ShellRoutes.ChatView + $"user={json}");
        }

        private async void setUsers()
        {
            ChatDatabase.ChatDatabase database = AppManager.Instance.Services.GetService(typeof(ChatDatabase.ChatDatabase)) as ChatDatabase.ChatDatabase;
            List<ChatUser> users = await database.GetChatUsers();
            Users = new ObservableCollection<ChatUser>(users.OrderBy(user => user.LastMessage));
        }
        #endregion
    }
}
