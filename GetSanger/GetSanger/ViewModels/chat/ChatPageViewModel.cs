using GetSanger.Models;
using GetSanger.Models.chat;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;
using GetSanger.ChatDatabase;

namespace GetSanger.ViewModels.chat
{
    [QueryProperty(nameof(UserToChat), "userTo")]
    public class ChatPageViewModel : BaseViewModel
    {
        #region Fields
        private string m_TextToSend;
        private User m_UserToChat;
        private bool m_ShowScrollTap;
        private bool m_LastMessageVisible;
        private ObservableCollection<Message> m_MessagesSource;
        #endregion

        #region Properties
        public string TextToSend
        {
            get => m_TextToSend;
            set => SetClassProperty(ref m_TextToSend, value);
        }

        public User UserToChat
        {
            get => m_UserToChat;
            set => SetClassProperty(ref m_UserToChat, value);
        }

        public bool ShowScrollTap
        {
            get => m_ShowScrollTap;
            set => SetStructProperty(ref m_ShowScrollTap, value);
        }

        public bool LastMessageVisible
        {
            get => m_LastMessageVisible;
            set => SetStructProperty(ref m_LastMessageVisible, value);
        }

        public ChatDatabase.ChatDatabase DB { get; set; }

        public ObservableCollection<Message> MessagesSource
        {
            get => m_MessagesSource;
            set => SetClassProperty(ref m_MessagesSource, value);
        }

        public int PendingMessageCount { get; set; }

        public bool PendingMessageCountVisible { get { return PendingMessageCount > 0; } }

        public Queue<Message> DelayedMessages { get; set; }
        #endregion

        #region Commands
        public ICommand SendMessageCommand { get; set; }
        public ICommand MessageAppearingCommand { get; set; }
        public ICommand MessageDisappearingCommand { get; set; }
        #endregion

        #region Constructor
        public ChatPageViewModel()
        {
            SendMessageCommand = new Command(sendMessage);
            MessageAppearingCommand = new Command(messageAppearing);
            MessageDisappearingCommand = new Command(messageDisappearing);
            DB = (ChatDatabase.ChatDatabase)AppManager.Instance.Services.GetService(typeof(ChatDatabase.ChatDatabase));
            AppManager.Instance.ConnectedUser = new User
            {
                UserID = "Maor",
                PersonalDetails = new PersonalDetails
                {
                    NickName = "Maor"
                }
            };

            UserToChat = new User
            {
                UserID = "Yossi",
                PersonalDetails = new PersonalDetails
                {
                    NickName = "Yossi"
                }
            };
        }
        #endregion

        #region Methods
        public async override void Appearing()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            List<Message> messages = await DB.GetItemsAsync(UserToChat.UserID);
            MessagesSource = new ObservableCollection<Message>((from item in messages
                                                                select item).ToList()
                                                               );
            ShowScrollTap = false;
            DelayedMessages = new Queue<Message>();
            PendingMessageCount = 0;
            LastMessageVisible = true;
        }

        public void Disappearing()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        private async void sendMessage(object i_Param)
        {
            if (!string.IsNullOrWhiteSpace(TextToSend))
            {
                Message msg = new Message
                {
                    Text = TextToSend,
                    FromId = AppManager.Instance.ConnectedUser.UserID,
                    ToId = UserToChat.UserID,
                    TimeSent = DateTime.Now
                };

                // try catch for no internet
                MessagesSource.Insert(0, msg);
                await RunTaskWhileLoading(r_PushService.SendToDevice(msg.ToId, msg, msg.GetType(), "Message received", $"{ AppManager.Instance.ConnectedUser.PersonalDetails.NickName} sent you a message."));
                // adding the message to the local DB
                msg.MessageSent = true; // removing the '!' in the UI
                await DB.SaveItemAsync(msg, msg.ToId);
                TextToSend = string.Empty;
            }
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
           if(Connectivity.NetworkAccess == NetworkAccess.Internet) // there is an internet and there are messages not sent
           {
                foreach(Message msg in MessagesSource.Reverse())
                {
                    if(msg.MessageSent == false)
                    {
                        await RunTaskWhileLoading(r_PushService.SendToDevice(msg.ToId, msg, msg.GetType(), "Message received", $"{ AppManager.Instance.ConnectedUser.PersonalDetails.NickName} sent you a message."));
                        await DB.UpdateSentItemAsync(msg, msg.ToId);
                        msg.MessageSent = true;
                    }
                }
           }
        }

        private void messageAppearing(object i_Param)
        {
            Message message = i_Param as Message;
            var idx = MessagesSource.IndexOf(message);
            if (idx <= 6)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    while (DelayedMessages.Count > 0)
                    {
                        MessagesSource.Insert(0, DelayedMessages.Dequeue());
                    }
                    ShowScrollTap = false;
                    LastMessageVisible = true;
                    PendingMessageCount = 0;
                });
            }
        }

        private void messageDisappearing(object i_Param)
        {
            Message message = i_Param as Message;
            var idx = MessagesSource.IndexOf(message);
            if (idx >= 6)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ShowScrollTap = true;
                    LastMessageVisible = false;
                });

            }
        }
        #endregion
    }
}
