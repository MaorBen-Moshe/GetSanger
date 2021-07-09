using GetSanger.Models;
using GetSanger.Models.chat;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;
using GetSanger.Extensions;
using GetSanger.Constants;

namespace GetSanger.ViewModels.chat
{
    [QueryProperty(nameof(UserJson), "user")]
    public class ChatPageViewModel : BaseViewModel
    {
        #region Fields
        private string m_TextToSend;
        private User m_UserToChat;
        private bool m_ShowScrollTap;
        private bool m_LastMessageVisible;
        private bool m_PendingMessageCountVisible;
        private int m_PendingMessageCount;
        private Queue<Message> m_DelayedMessages;
        private ObservableCollection<Message> m_MessagesSource;
        private ImageSource m_UserPicture;
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

        public string UserJson
        {
            set
            {
                UserToChat = ObjectJsonSerializer.DeserializeForPage<User>(value);
            }
        }

        public ImageSource UserPicture
        {
            get => m_UserPicture;
            set => SetClassProperty(ref m_UserPicture, value);
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

        public ObservableCollection<Message> MessagesSource
        {
            get => m_MessagesSource;
            set => SetClassProperty(ref m_MessagesSource, value);
        }

        public int PendingMessageCount
        {
            get => m_PendingMessageCount;
            set => SetStructProperty(ref m_PendingMessageCount, value);
        }

        public bool PendingMessageCountVisible
        {
            get => m_PendingMessageCountVisible;
            set => SetStructProperty(ref m_PendingMessageCountVisible, value);
        }

        public Queue<Message> DelayedMessages
        {
            get => m_DelayedMessages;
            set => SetClassProperty(ref m_DelayedMessages, value);
        }

        private ChatDatabase.ChatDatabase DB { get; set; }
        #endregion

        #region Commands
        public ICommand SendMessageCommand { get; set; }
        public ICommand MessageAppearingCommand { get; set; }
        public ICommand MessageDisappearingCommand { get; set; }
        public ICommand DeleteMessageCommand { get; set; }
        public ICommand SendWhatsappCommand { get; set; }
        public ICommand CallCommand { get; set; }
        public ICommand HandleMessageReceivedCommand { get; set; }
        public ICommand ClickProfileBarCommand { get; set; }
        #endregion

        #region Constructor
        public ChatPageViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods
        public async override void Appearing()
        {
            try
            {
                r_CrashlyticsService.LogPageEntrance(nameof(ChatPageViewModel));
                UserPicture = r_PhotoDisplay.DisplayPicture(UserToChat.ProfilePictureUri);
                DB = await ChatDatabase.ChatDatabase.Instance;
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                setMessages();
                ShowScrollTap = false;
                DelayedMessages = new Queue<Message>();
                PendingMessageCount = 0;
                PendingMessageCountVisible = false;
                LastMessageVisible = true;
            }
            catch(Exception e)
            {
                await GoBack();
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:Appearing", "Error", e.Message);
            }
        }

        public void Disappearing()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        private void setCommands()
        {
            SendMessageCommand = new Command(sendMessage);
            MessageAppearingCommand = new Command(messageAppearing);
            MessageDisappearingCommand = new Command(messageDisappearing);
            DeleteMessageCommand = new Command(deleteMessage);
            SendWhatsappCommand = new Command(sendWhatsapp);
            CallCommand = new Command(call);
            HandleMessageReceivedCommand = new Command(handleMessageReceived);
            ClickProfileBarCommand = new Command(clickProfileBar);
        }

        private async void handleMessageReceived(object i_Param)
        {
            try
            {
                if(i_Param is Message message)
                {
                    if (LastMessageVisible)
                    {
                        MessagesSource.Insert(0, message);
                    }
                    else
                    {
                        DelayedMessages.Enqueue(message);
                        PendingMessageCount++;
                        PendingMessageCountVisible = true;
                    }
                }
            }
            catch (Exception e)
            {
                await GoBack();
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:handleMessageReceived", "Error", e.Message);
            }
        }

        private async void setMessages()
        {
            List<Message> messages = await DB.GetMessagesAsync(UserToChat.UserId);
            MessagesSource = new ObservableCollection<Message>(messages.OrderByDescending(item => item.MessageId));
            sendUnsentMessages();
        }

        private async void sendMessage(object i_Param)
        {
            if (!string.IsNullOrWhiteSpace(TextToSend))
            {
                Message msg = new Message
                {
                    Text = TextToSend,
                    FromId = AppManager.Instance.ConnectedUser.UserId,
                    ToId = UserToChat.UserId,
                    TimeSent = DateTime.Now,
                    MessageSent = false
                };

                TextToSend = string.Empty;
                try
                {
                    MessagesSource.Insert(0, msg);
                    r_PushService.SendChatMessage(msg);
                    // adding the message to the local DB
                    msg.MessageSent = true;
                    await DB.AddMessageAsync(msg, msg.ToId);
                }
                catch(Exception e)
                {
                    msg.MessageSent = false;
                    await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:sendMessage", "Error", e.Message);
                }
            }
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            sendUnsentMessages();
        }

        private async void sendUnsentMessages()
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet) // there is an internet and there are messages not sent
                {
                    if(MessagesSource != null)
                    {
                        foreach (Message msg in MessagesSource)
                        {
                            if (msg.MessageSent == false)
                            {
                                r_PushService.SendChatMessage(msg);
                                msg.MessageSent = true;
                                await DB.UpdateMessageAsync(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:sendUnsentMessages", "Error", e.Message);
            }
        }

        private async void deleteMessage(object i_Param)
        {
            try
            {
                await r_PageService.DisplayAlert("Note",
                                                 "Are you sure?\nMessage will be deleted on your device only.",
                                                 "Yes",
                                                 "No",
                                                 async (answer) =>
                                                 {
                                                     if (answer)
                                                     {
                                                         Message message = i_Param as Message;
                                                         await DB.DeleteMessageAsync(message);
                                                         MessagesSource.Remove(message);
                                                     }
                                                 });
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:deleteMessage", "Error", "Something went wrong!");
            }
        }

        private async void messageAppearing(object i_Param)
        {
            try
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
                        PendingMessageCountVisible = false;
                    });
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:messageAppearing", "Error", e.Message);
            }
        }

        private async void messageDisappearing(object i_Param)
        {
            try
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
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:messageDisappearing", "Error", e.Message);
            }
        }

        private async void sendWhatsapp(object i_Param)
        {
            try
            {
                if (r_DialService.IsValidPhone(UserToChat.PersonalDetails.Phone))
                {
                    r_DialService.PhoneNumber = UserToChat.PersonalDetails.Phone;
                    bool succeeded = await r_DialService.SendWhatsapp();
                    if (!succeeded)
                    {
                        await r_DialService.SendDefAppMsg();
                    }

                    return;
                }

                await r_PageService.DisplayAlert("Note", "User does not provide phone number!", "OK");
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:sendWhatsapp", "Error", e.Message);
            }
        }

        private async void call(object i_Param)
        {
            try
            {
                if (r_DialService.IsValidPhone(UserToChat.PersonalDetails.Phone))
                {
                    r_DialService.PhoneNumber = UserToChat.PersonalDetails.Phone;
                    r_DialService.Call();
                    return;
                }

                await r_PageService.DisplayAlert("Note", "User does not provide phone number!", "OK");
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:call", "Error", e.Message);
            }
        }

        private async void clickProfileBar(object i_Param)
        {
            try
            {
                await r_NavigationService.NavigateTo($"{ShellRoutes.Profile}?userid={UserToChat.UserId}");
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:clickProfileBar", "Error", e.Message);
            }
        }
        #endregion
    }
}