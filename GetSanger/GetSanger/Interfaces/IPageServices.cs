using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    interface IPageService
    {
        Task PushAsync(Page i_Page);

        Task<bool> DisplayAlert(string i_Title, string i_Message, string i_Accept, string i_Cancel);

        Task PopAsync();
    }
}
