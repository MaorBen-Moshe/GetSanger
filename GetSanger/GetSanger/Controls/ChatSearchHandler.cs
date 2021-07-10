using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Models.chat;
using GetSanger.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class ChatSearchHandler : BaseSearchHandler<ChatUser>
    {
        #region Properties
        #endregion

        #region Methods

        protected async override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ObservableCollection<ChatUser> newSource = new ObservableCollection<ChatUser>();
                foreach(var current in Source)
                {
                    User user = await FireStoreHelper.GetUser(current.UserId);
                    if (user.PersonalDetails.NickName.ToLower().Contains(newValue.ToLower()))
                    {
                        newSource.Add(current);
                    }
                }

                ItemsSource = newSource;
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);

            // Let the animation complete
            await Task.Delay(1000);

            ShellNavigationState state = (App.Current.MainPage as Shell).CurrentState;
            // The following route works because route names are unique in this application.
            string json = ObjectJsonSerializer.SerializeForPage(await FireStoreHelper.GetUser(((ChatUser)item).UserId));
            await Shell.Current.GoToAsync(ShellRoutes.ChatView + $"?user={json}&prev={ShellRoutes.ChatsList}");
        }

        #endregion
    }
}
