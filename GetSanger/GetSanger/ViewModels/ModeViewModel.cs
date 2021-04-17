using GetSanger.AppShell;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class ModeViewModel : BaseViewModel
    {
        public ICommand UserCommand { get; private set; }

        public ICommand SangerCommand { get; private set; }

        public ICommand BackButtonBehaviorCommand { get; set; }

        public ModeViewModel()
        {
            UserCommand = new Command(userCommandHelper);
            SangerCommand = new Command(sangerCommandHelper);
            BackButtonBehaviorCommand = new Command(backButtonBehavior);
        }

        private void backButtonBehavior(object i_Param)
        {
            // back to login page
            // set back the first login of this user to true
        }

        private void userCommandHelper()
        {
            App.Current.MainPage = new UserShell();
        }

        private void sangerCommandHelper()
        {
            App.Current.MainPage = new SangerShell();
        }
    }
}
