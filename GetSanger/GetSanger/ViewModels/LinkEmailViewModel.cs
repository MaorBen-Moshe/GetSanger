﻿using GetSanger.Extensions;
using GetSanger.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class LinkEmailViewModel : PopupBaseViewModel
    {
        #region Fields
        private string m_Email;
        private string m_Password;
        private string m_ConfirmPassword;
        #endregion

        #region Properties
        public string Email
        {
            get => m_Email;
            set => SetClassProperty(ref m_Email, value);
        }

        public string Password
        {
            get => m_Password;
            set => SetClassProperty(ref m_Password, value);
        }

        public string ConfirmPassword
        {
            get => m_ConfirmPassword;
            set => SetClassProperty(ref m_ConfirmPassword, value);
        }
        #endregion

        #region Commands

        public ICommand LinkCommand { get; set; }

        #endregion

        #region Constructor
        public LinkEmailViewModel()
        {
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(LinkEmailViewModel));
        }

        public override void Disappearing()
        {
            Email = Password = null;
        }

        protected override void SetCommands()
        {
            LinkCommand = new Command(link);
        }

        private async void link(object i_Param)
        {
            try
            {
                sr_LoadingService.ShowLoadingPage();
                if (Email != null && Password != null && ConfirmPassword.Equals(Password) && Password.IsValidPassword())
                {
                    await AuthHelper.LinkWithEmailAndPassword(Email, Password);
                    await sr_PageService.DisplayAlert("Success", $"Your account has linked with {Email}", "Thanks");
                    sr_LoadingService.HideLoadingPage();
                    await GoBack();
                }
                else
                {
                    await sr_PageService.DisplayAlert("Error", "Please fill Email And Password", "OK");
                }
            }
            catch(Exception e)
            {
                sr_LoadingService.HideLoadingPage();
                await e.LogAndDisplayError($"{nameof(LinkEmailViewModel)}:link", "Error", e.Message);
            }
        }
        #endregion
    }
}