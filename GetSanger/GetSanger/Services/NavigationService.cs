using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class NavigationService
    {
        public NavigationService()
        {

        }

        public void OpenMainPage()
        {

        }

        public async void GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task NavigateTo(string i_ViewPath)
        {
            ShellNavigationState state = Shell.Current.CurrentState;
            try
            {
                await Shell.Current.GoToAsync(i_ViewPath);
            }
            catch
            {
                throw;
            }
        }
    }
}
