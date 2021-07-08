using GetSanger.ViewModels.chat;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatView : ContentPage
    {
        public ChatView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as ChatPageViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as ChatPageViewModel).Disappearing();
            base.OnDisappearing();
        }

        public void ScrollTap(object sender, System.EventArgs e)
        {
            lock (new object())
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