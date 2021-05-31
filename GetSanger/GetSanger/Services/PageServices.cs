using GetSanger.Interfaces;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    class PageServices : Service, IPageService
    {
        public async Task<bool> DisplayAlert(string i_Title, string i_Message, string i_Accept, string i_Cancel = null)
        {
            if(i_Cancel == null)
            {
                await Application.Current.MainPage.DisplayAlert(i_Title, i_Message, i_Accept);
                return true;
            }

            return await Application.Current.MainPage.DisplayAlert(i_Title, i_Message, i_Accept, i_Cancel);
        }

        public override void SetDependencies()
        {
            //;
        }

        public async Task<string> DisplayActionSheet(string i_Title, string i_Cancel, string i_Distruction, params string[] i_Buttons)
        {
            return await Application.Current.MainPage.DisplayActionSheet(i_Title, i_Cancel, i_Distruction, i_Buttons);
        }
    }
}
