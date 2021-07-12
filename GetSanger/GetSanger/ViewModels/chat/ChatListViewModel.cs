﻿using GetSanger.Constants;
using GetSanger.Extensions;
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
            setCommands();
        }

        #endregion

        #region Methods

        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(ChatListViewModel));
            setUsers();
        }

        protected override void setCommands()
        {
            base.setCommands();
            UserSelectedCommand = new Command(userSelected);
        }

        protected async override void refreshList()
        {
            try
            {
                setUsers();
                IsListRefreshing = false;
            }catch(Exception e)
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
                    await r_NavigationService.NavigateTo($"{ShellRoutes.ChatView}?user={json}&prev={ShellRoutes.ChatsList}");
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

        private async void setUsers()
        {
            r_LoadingService.ShowLoadingPage();
            ChatDatabase.ChatDatabase database = await ChatDatabase.ChatDatabase.Instance;
            List<ChatUser> users = (await database.GetAllUsersAsync()).ToList();
            foreach(var user in users)
            {
                user.User = await FireStoreHelper.GetUser(user.UserId);
            }

            AllCollection = new ObservableCollection<ChatUser>(users.OrderByDescending(user => user.LastMessage));
            SearchCollection = new ObservableCollection<ChatUser>(AllCollection);
            IsVisibleViewList = AllCollection.Count > 0;
            r_LoadingService.HideLoadingPage();
        }

        protected override void filterSelected(object i_Param)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}