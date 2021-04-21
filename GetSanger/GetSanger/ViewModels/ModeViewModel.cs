using GetSanger.AppShell;
using GetSanger.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class ModeViewModel : BaseViewModel
    {
        #region Commands
        public ICommand UserCommand { get; private set; }

        public ICommand SangerCommand { get; private set; }

        #endregion

        #region Constructor
        public ModeViewModel()
        {
            UserCommand = new Command(userCommandHelper);
            SangerCommand = new Command(sangerCommandHelper);
        }
        #endregion

        #region Methods
        private void userCommandHelper()
        {
            setUserMode(AppMode.Client, new UserShell());
        }

        private void sangerCommandHelper()
        {
            setUserMode(AppMode.Sanger, new SangerShell());
        }

        private async void setUserMode(AppMode i_Mode, Shell i_ChosenShell)
        {
            if(await AuthHelper.IsVerifiedEmail() == false)
            {
                await r_PageService.DisplayAlert("Note", "Please verify your email address to continue!", "OK");
                return;
            }

            AppManager.Instance.CurrentMode = i_Mode;
            AppManager.Instance.ConnectedUser.LastUserMode = i_Mode;
            await RunTaskWhileLoading(FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser));
            Application.Current.MainPage = i_ChosenShell;
        }

        public override void Appearing()
        {
        }
        #endregion
    }
}
