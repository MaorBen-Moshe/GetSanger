using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class NavigationService : Service, Interfaces.INavigation
    { 
        public async Task NavigateTo(string i_ViewPath)
        {
            try
            {
                await Shell.Current.GoToAsync(i_ViewPath);
            }
            catch
            {
                throw;
            }
        }

        protected override void SetDependencies()
        {
        }
    }
}