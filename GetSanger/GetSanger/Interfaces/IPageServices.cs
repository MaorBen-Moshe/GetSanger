using System;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IPageService
    {
        // if i_Acccept is null the alert is displayed for 1.5 seconds and than disappears by itself else it await for response from the user
        Task DisplayAlert(string i_Title,
                          string i_Message,
                          string i_Accept = null,
                          string i_Cancel = null,
                          Action<bool> UserChoseOptionAction = null);

        Task<string> DisplayActionSheet(string i_Title, string i_Cancel, string i_Distruction, params string[] i_Buttons);

        public void DisplayPrompt(string i_Title, string i_Message, string i_PlaceHolder, Action<string> i_RetAction);
    }
}