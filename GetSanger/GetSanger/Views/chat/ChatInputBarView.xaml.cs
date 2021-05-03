using GetSanger.ViewModels.chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatInputBarView : ContentView
    {
        public ChatInputBarView()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                SetBinding(HeightRequestProperty, new Binding("Height", BindingMode.OneWay, null, null, null, chatTextInput));
            }
        }

        public void Handle_Completed(object sender, EventArgs e)
        {
            (this.Parent.Parent.BindingContext as ChatPageViewModel).SendMessageCommand.Execute(null);
            chatTextInput.Focus();
        }

        public void UnFocusEntry()
        {
            chatTextInput?.Unfocus();
        }
    }
}