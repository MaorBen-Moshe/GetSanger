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
    public partial class ChatsListView : ContentPage
    {
        public ChatsListView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as ChatListViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as ChatListViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}