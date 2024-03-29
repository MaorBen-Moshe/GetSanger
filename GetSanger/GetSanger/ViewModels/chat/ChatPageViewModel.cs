﻿using GetSanger.Models;
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
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using GetSanger.Interfaces;
using System.Threading.Tasks;

namespace GetSanger.ViewModels.chat
{
    [QueryProperty(nameof(UserJson), "user")]
    [QueryProperty(nameof(PrevPage), "prev")]
    public class ChatPageViewModel : BaseViewModel
    {
        #region Fields
        private string m_TextToSend;
        private User m_UserToChat;
        private bool m_ShowScrollTap;
        private bool m_LastMessageVisible;
        private bool m_PendingMessageCountVisible;
        private bool m_IsDeletedUser;
        private int m_PendingMessageCount;
        private Queue<Message> m_DelayedMessages;
        private ObservableCollection<Message> m_MessagesSource;
        private ImageSource m_UserPicture;
        private Message m_SelectedItem;
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

        public string PrevPage { get; set; }

        public bool IsDeletedUser
        {
            get => m_IsDeletedUser;
            set => SetStructProperty(ref m_IsDeletedUser, value);
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

        public Message SelectedItem
        {
            get => m_SelectedItem;
            set => SetClassProperty(ref m_SelectedItem, null);
        }

        private IChatDb DB { get; set; }
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
        }
        #endregion

        #region Methods
        public async override void Appearing()
        {
            try
            {
                sr_CrashlyticsService.LogPageEntrance(nameof(ChatPageViewModel));
                UserPicture = sr_PhotoDisplay.DisplayPicture(UserToChat.ProfilePictureUri);
                IsDeletedUser = UserToChat.IsDeleted;
                DB = await ChatDatabase.ChatDatabase.Instance;
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                ShowScrollTap = false;
                DelayedMessages = new Queue<Message>();
                PendingMessageCount = 0;
                PendingMessageCountVisible = false;
                LastMessageVisible = true;
                setMessages();
                if (Device.RuntimePlatform.Equals(Device.Android))
                {
                    Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
                }
            }
            catch(Exception e)
            {
                await GoBack();
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:Appearing", "Error", e.Message);
            }
        }

        public override void Disappearing()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
            }
        }

        protected override void SetCommands()
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
            await Task.Run(() => sendUnsentMessages());
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
                    await Task.Run(() =>
                    {
                        sr_PushService.SendChatMessage(msg);
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            msg.MessageSent = true;
                            await DB.AddMessageAsync(msg, msg.ToId);
                        });
                    });
                }
                catch(Exception e)
                {
                    await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:sendMessage", "Error", e.Message);
                }
            }
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            await Task.Run(() => sendUnsentMessages());
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
                                sr_PushService.SendChatMessage(msg);
                                Device.BeginInvokeOnMainThread(async () => 
                                {
                                    msg.MessageSent = true;
                                    await DB.UpdateMessageAsync(msg);
                                });
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
                await sr_PageService.DisplayAlert("Note",
                                                 "Are you sure?\nMessage will be deleted on your device only.",
                                                 "Yes",
                                                 "No",
                                                 async (answer) =>
                                                 {
                                                     if (answer)
                                                     {
                                                         if(i_Param is Message message)
                                                         {
                                                             MessagesSource.Remove(message);
                                                             await Task.Run(async () => await DB.DeleteMessageAsync(message));
                                                         }
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
                if(i_Param is Message message)
                {
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
                if(i_Param is Message message)
                {
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
                if (sr_DialService.IsValidPhone(UserToChat.PersonalDetails.Phone))
                {
                    sr_DialService.PhoneNumber = UserToChat.PersonalDetails.Phone;
                    bool succeeded = await sr_DialService.SendWhatsapp();
                    if (!succeeded)
                    {
                        await sr_DialService.SendDefAppMsg();
                    }

                    return;
                }

                await sr_PageService.DisplayAlert("Note", "User did not provide phone number!", "OK");
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
                if (sr_DialService.IsValidPhone(UserToChat.PersonalDetails.Phone))
                {
                    sr_DialService.PhoneNumber = UserToChat.PersonalDetails.Phone;
                    sr_DialService.Call();
                    return;
                }

                await sr_PageService.DisplayAlert("Note", "User did not provide phone number!", "OK");
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
                if (PrevPage.Equals(ShellRoutes.Profile))
                {
                    await GoBack();
                }
                else
                {
                    await sr_NavigationService.NavigateTo($"{ShellRoutes.Profile}?userid={UserToChat.UserId}");
                }
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ChatPageViewModel)}:clickProfileBar", "Error", e.Message);
            }
        }
        #endregion
    }
}