using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface IPageService
    {
        Task<bool> DisplayAlert(string i_Title, string i_Message, string i_Accept, string i_Cancel = null);

        Task<string> DisplayActionSheet(string i_Title, string i_Cancel, string i_Distruction, params string[] i_Buttons);
    }
}
