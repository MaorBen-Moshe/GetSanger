﻿using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface IPopupService
    {
        void InitPopupgPage(ContentPage i_PopupPage = null);

        void ShowPopupgPage();

        void HidePopupPage();
    }
}