﻿using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public enum SocialProvider { Facebook, Google, Apple, Email } // email is for the link only
    public class SocialAdapterService : Service
    {
        private LoginServices m_LoginService;
        private NavigationService m_NavigationService;

        public async Task<bool> SocialLogin(SocialProvider i_Provider)
        {
            if(m_LoginService == null || m_NavigationService == null)
            {
                SetDependencies();
            }

            bool succeeded = true;
            Dictionary<string, object> details = await AuthHelper.LoginWithProvider(i_Provider);
            bool isFirstLoggedin = await AuthHelper.IsFirstTimeLogIn();
            if (isFirstLoggedin)
            {
                string json = ObjectJsonSerializer.SerializeForPage(getDetails(details));
                //AppManager.Instance.SignUpVM.UserJson = ObjectJsonSerializer.SerializeForServer(getDetails(details));
                string route = $"{ShellRoutes.SignupPersonalDetails}?isFacebookGmail={true}&userJson={json}";
                await m_NavigationService.NavigateTo(route);
            }
            else
            {
                succeeded = await m_LoginService.LoginUser(socialLogin:true);
            }

            return succeeded;
        }

        private User getDetails(Dictionary<string, object> i_Details)
        {
            User user = new User
            {
                PersonalDetails = new PersonalDetails
                {
                    NickName = i_Details["displayName"] as string
                },
                Email = i_Details["email"] as string
            };

            if (i_Details["photoUrl"] != null)
            {
                user.ProfilePictureUri = new Uri(i_Details["photoUrl"] as string);
            }

            return user;
        }

        public override void SetDependencies()
        {
            m_LoginService = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            m_NavigationService = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
        }
    }
}
