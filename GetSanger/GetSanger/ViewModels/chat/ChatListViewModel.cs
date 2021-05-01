using GetSanger.Constants;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels.chat
{
    public class ChatListViewModel : BaseViewModel
    {
        #region Fields
        #endregion

        #region Properties
        public ObservableCollection<User> Users { get; set; }
        #endregion

        #region Commands
        public ICommand UserSelectedCommand { get; set; }
        #endregion

        #region Constructor
        public ChatListViewModel()
        {
            UserSelectedCommand = new Command(userSelected);
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            // get all the users the connected has chat with them
            // also need to implement the order between them
        }

        private async void userSelected(object i_Param)
        {
            await r_NavigationService.NavigateTo(ShellRoutes.ChatView + $"?userTo={i_Param as User}");
        }
        #endregion
    }
}
