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
    public class ChatListViewModel : ListBaseViewModel<ChatUser>
    {
        #region Fields

        #endregion

        #region Properties

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

        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(ChatListViewModel));
            setUsers();
        }

        private void setCommands()
        {
            UserSelectedCommand = new Command(userSelected);
        }

        protected override void refreshList()
        {
            setUsers();
            IsListRefreshing = false;
        }

        private async void userSelected(object i_Param)
        {
            string json = ObjectJsonSerializer.SerializeForPage((i_Param as ChatUser).User);
            await r_NavigationService.NavigateTo($"{ShellRoutes.ChatView}?user={json}");
        }

        private async void setUsers()
        {
            r_LoadingService.ShowPopup();
            ChatDatabase.ChatDatabase database = await ChatDatabase.ChatDatabase.Instance;
            List<ChatUser> users = (await database.GetAllUsersAsync()).ToList();
            foreach(var user in users)
            {
                user.User = await FireStoreHelper.GetUser(user.UserId);
            }

            Collection = new ObservableCollection<ChatUser>(users.OrderByDescending(user => user.LastMessage));
            SearchCollection = new ObservableCollection<ChatUser>(Collection);
            IsVisibleViewList = Collection.Count > 0;
            r_LoadingService.HidePopup();
        }

        #endregion
    }
}