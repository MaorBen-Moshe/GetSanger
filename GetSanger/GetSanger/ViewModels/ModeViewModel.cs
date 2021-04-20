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
            setUserMode(AppMode.Client);
            App.Current.MainPage = new UserShell();
        }

        private void sangerCommandHelper()
        {
            setUserMode(AppMode.Sanger);
            App.Current.MainPage = new SangerShell();
        }

        private async void setUserMode(AppMode i_Mode)
        {
            AppManager.Instance.CurrentMode = i_Mode;
            AppManager.Instance.ConnectedUser.LastUserMode = i_Mode;
            await RunTaskWhileLoading(FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser));
        }

        protected override void appearing(object i_Param)
        {
        }

        protected override void disappearing(object i_Param)
        {
        }
        #endregion
    }
}
