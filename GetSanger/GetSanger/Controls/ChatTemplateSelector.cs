using GetSanger.Models.chat;
using GetSanger.Services;
using GetSanger.Views.chat;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;

        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as Message;
            if (messageVm == null)
                return null;


            return (messageVm.FromId == AppManager.Instance.ConnectedUser.UserID) ? outgoingDataTemplate : incomingDataTemplate;
        }

    }
}
