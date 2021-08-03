using GetSanger.Interfaces;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    class PageServices : Service, IPageService
    {
        public async Task DisplayAlert(string i_Title, 
                                       string i_Message, 
                                       string i_Accept = null, 
                                       string i_Cancel = null, 
                                       Action<bool> UserChoseOptionAction = null)
        {
            if(i_Accept == null && i_Cancel != null)
            {
                throw new ArgumentException("i_Accept param can be null only if i_Cancel param is set to null");
            }

            var page = new DisplayAlertPage(i_Title, i_Message, i_Accept, i_Cancel, UserChoseOptionAction);
            await PopupNavigation.Instance.PushAsync(page);
            if(i_Accept == null)
            {
                await Task.Delay(2000);
                await PopupNavigation.Instance.PopAsync();
            }
        }

        public  Task<string> DisplayActionSheet(string i_Title, string i_Cancel, string i_Distruction, params string[] i_Buttons)
        {
            return Application.Current.MainPage.DisplayActionSheet(i_Title, i_Cancel, i_Distruction, i_Buttons);
        }

        public Task<string> DisplayPrompt(string i_Title, string i_Message, string i_PlaceHolder)
        {
            var keyboard = Keyboard.Create(KeyboardFlags.All);
            return Application.Current.MainPage.DisplayPromptAsync(i_Title, i_Message, placeholder: i_PlaceHolder, keyboard:keyboard);
        }

        public override void SetDependencies()
        {
        }
    }
}