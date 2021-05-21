using GetSanger.Services;
using System;
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
            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
        }

        private void setCommands()
        {
            UserCommand = new Command(userCommandHelper);
            SangerCommand = new Command(sangerCommandHelper);
        }

        private void userCommandHelper()
        {
            try
            {
                r_LoginServices.LoginUser(AppMode.Client);
            }
            catch(Exception e)
            {
                handleException(e);
            }
        }

        private void sangerCommandHelper()
        {
            try
            {
                r_LoginServices.LoginUser(AppMode.Sanger);
            }
            catch(Exception e)
            {
                handleException(e);
            }
        }


        private async void handleException(Exception e)
        {
            string message = e.Message;
            if (e.InnerException != null)
            {
                message = string.Format("{0} \n {1}", message, e.InnerException.Message);
            }

            await r_PageService.DisplayAlert("Error", message, "OK");
        }
        #endregion
    }
}
