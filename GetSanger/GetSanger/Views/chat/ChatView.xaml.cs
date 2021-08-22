using GetSanger.ViewModels.chat;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatView : ContentPage
    {
        private static readonly object sr_ScrollLock;

        public ICommand ScrollCommand;

        static ChatView()
        {
            sr_ScrollLock = new object();
        }

        public ChatView()
        {
            InitializeComponent();

            ScrollCommand = new Command(() => ChatList?.ScrollToFirst());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as ChatPageViewModel).Appearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as ChatPageViewModel).Disappearing();
        }

        public void ScrollTap(object sender, System.EventArgs e)
        {
            lock (sr_ScrollLock)
            {
                if (BindingContext != null)
                {
                    var vm = BindingContext as ChatPageViewModel;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        while (vm.DelayedMessages.Count > 0)
                        {
                            vm.MessagesSource.Insert(0, vm.DelayedMessages.Dequeue());
                        }

                        vm.ShowScrollTap = false;
                        vm.LastMessageVisible = true;
                        vm.PendingMessageCount = 0;
                        ChatList?.ScrollToFirst();
                    });
                }
            }
        }

        public void OnListTapped(object sender, ItemTappedEventArgs e)
        {
            chatInput.UnFocusEntry();
        }
    }
}