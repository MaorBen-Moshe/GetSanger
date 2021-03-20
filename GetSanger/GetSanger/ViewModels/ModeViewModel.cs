using GetSanger.AppShell;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class ModeViewModel : BaseViewModel
    {
        public ICommand UserCommand { get; private set; }

        public ICommand SangerCommand { get; private set; }

        public ModeViewModel()
        {
            UserCommand = new Command(userCommandHelper);
            SangerCommand = new Command(sangerCommandHelper);
        }

        private void userCommandHelper()
        {
            Shell.Current.GoToAsync("/AppShell/UserShell");
        }

        private void sangerCommandHelper()
        {
            Shell.Current.GoToAsync("/AppShell/SangerShell");
        }
    }
}
