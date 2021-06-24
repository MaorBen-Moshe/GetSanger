﻿using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class LinkEmailViewModel : BaseViewModel
    {
        #region Fields
        private string m_Email;
        private string m_password;
        #endregion

        #region Properties
        public string Email
        {
            get => m_Email;
            set => SetClassProperty(ref m_Email, value);
        }

        public string Password
        {
            get => m_Email;
            set => SetClassProperty(ref m_password, value);
        }
        #endregion

        #region Commands

        public ICommand LinkCommand { get; set; }

        #endregion

        #region Constructor
        public LinkEmailViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
        }

        public void Disappearing()
        {
            Email = Password = null;
        }

        private void setCommands()
        {
            LinkCommand = new Command(link);
        }

        private async void link(object i_Param)
        {
            try
            {
                r_LoadingService.ShowPopup();
                if (Email != null && Password != null)
                {
                    await AuthHelper.LinkWithEmailAndPassword(Email, Password);
                    await r_PageService.DisplayAlert("Success", $"Your account has linked with {Email}", "Thanks");
                    await GoBack();
                }
                else
                {
                    await r_PageService.DisplayAlert("Error", "Please fill Email And Password", "OK");
                }
            }
            catch(Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                r_LoadingService.HidePopup();
            }
        }
        #endregion
    }
}