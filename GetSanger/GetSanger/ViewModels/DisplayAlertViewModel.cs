using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class DisplayAlertViewModel : PopupBaseViewModel
    {
        #region Fields
        private readonly Action<bool> r_UserChoseOptionAction;
        private string m_Header;
        private string m_Message;
        private string m_OkText;
        private string m_CancelText;
        private bool m_OkVisible;
        private bool m_CancelVisible;
        #endregion

        #region Properties
        public string Header
        {
            get => m_Header;
            set => SetClassProperty(ref m_Header, value);
        }

        public string Message
        {
            get => m_Message;
            set => SetClassProperty(ref m_Message, value);
        }

        public string OkText
        {
            get => m_OkText;
            set => SetClassProperty(ref m_OkText, value);
        }

        public string CancelText
        {
            get => m_CancelText;
            set => SetClassProperty(ref m_CancelText, value);
        }

        public bool OkVisible
        {
            get => m_OkVisible;
            set => SetStructProperty(ref m_OkVisible, value);
        }

        public bool CancelVisible
        {
            get => m_CancelVisible;
            set => SetStructProperty(ref m_CancelVisible, value);
        }
        #endregion

        #region Commands
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion

        #region Constructor
        public DisplayAlertViewModel(string i_Header,
                                     string i_Message,
                                     string i_OkText = "OK",
                                     string i_CancelText = null,
                                     Action<bool> i_Action = null)
        {
            Header = i_Header ?? "Error";
            Message = i_Message ?? "Something went wrong! :(";
            OkVisible = i_OkText != null;
            OkText = i_OkText;
            CancelText = i_CancelText;
            CancelVisible= i_CancelText != null;
            r_UserChoseOptionAction = i_Action;
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(DisplayAlertViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            OkCommand = new Command(() => helper(true));
            CancelCommand = new Command(() => helper(false));
        }

        private async void helper(bool i_OkClicked)
        {
            await PopupNavigation.Instance.PopAsync();
            r_UserChoseOptionAction?.Invoke(i_OkClicked);
        }
        #endregion
    }
}