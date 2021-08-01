using GetSanger.Models.chat;
using GetSanger.Services;
using GetSanger.Views.chat;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate r_IncomingDataTemplate;
        private readonly DataTemplate r_OutgoingDataTemplate;

        public ChatTemplateSelector()
        {
            r_IncomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            r_OutgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as Message;
            if (messageVm == null)
            {
                return null;
            }

            return (messageVm.FromId == AppManager.Instance.ConnectedUser.UserId) ? r_OutgoingDataTemplate : r_IncomingDataTemplate;
        }
    }
}