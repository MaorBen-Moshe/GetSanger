﻿using GetSanger.Interfaces;
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
                                       string i_Accept, 
                                       string i_Cancel = null, 
                                       Action<bool> UserChoseOptionAction = null)
        {
            var page = new DisplayAlertPage(i_Title, i_Message, i_Accept, i_Cancel, UserChoseOptionAction);
            await PopupNavigation.Instance.PushAsync(page);
        }

        public override void SetDependencies()
        {
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
    }
}
