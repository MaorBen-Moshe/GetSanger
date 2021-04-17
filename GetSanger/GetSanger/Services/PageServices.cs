using GetSanger.Interfaces;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    class PageServices : Service, IPageService
    {
        public async Task PushAsync(Page i_Page)
        {
            await Application.Current.MainPage.Navigation.PushAsync(i_Page);
        }

        public async Task<bool> DisplayAlert(string i_Title, string i_Message, string i_Accept, string i_Cancel = null)
        {
            if(i_Cancel == null)
            {
                await Application.Current.MainPage.DisplayAlert(i_Title, i_Message, i_Accept);
                return true;
            }

            return await Application.Current.MainPage.DisplayAlert(i_Title, i_Message, i_Accept, i_Cancel);
        }

        public async Task PopAsync()
        {
             await Application.Current.MainPage.Navigation.PopAsync();
        }

        public override void SetDependencies()
        {
            //;
        }
    }
}
