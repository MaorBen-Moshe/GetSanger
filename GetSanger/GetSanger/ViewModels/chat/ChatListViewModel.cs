using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Interfaces;
using GetSanger.Models.chat;
using GetSanger.Services;
using System;
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
        }

        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(ChatListViewModel));
            setUsers();
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            base.SetCommands();
            UserSelectedCommand = new Command(userSelected);
        }

        protected async override void refreshList()
        {
            try
            {
                setUsers();
                IsListRefreshing = false;
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatListViewModel)}:refreshList", "Error", e.Message);
            }
        }

        private async void userSelected(object i_Param)
        {
            try
            {
                if(i_Param is ChatUser chatUser)
                {
                    string json = ObjectJsonSerializer.SerializeForPage(chatUser.User);
                    await sr_NavigationService.NavigateTo($"{ShellRoutes.ChatView}?user={json}&prev={ShellRoutes.ChatsList}");
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatListViewModel)}:userSelected", "Error", e.Message);
            }
            finally
            {
                SelectedItem = null;
            }
        }

        private void setUsers()
        {
            setItems(async () =>
            {
                IChatDb database = await ChatDatabase.ChatDatabase.Instance;
                List<ChatUser> users = (await database.GetAllUsersAsync()).ToList();
                foreach (var user in users)
                {
                    user.User = await FireStoreHelper.GetUser(user.UserId);
                }

                AllCollection = new ObservableCollection<ChatUser>(users.OrderByDescending(user => user.LastMessage));
                SearchCollection = new ObservableCollection<ChatUser>(AllCollection);
                IsVisibleViewList = AllCollection.Count > 0;
                NoItemsText = "No chats available";
            });
        }

        protected override void filterSelected(object i_Param)
        {
            throw new NotImplementedException();
        }

        protected override void sort(object i_Param)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}