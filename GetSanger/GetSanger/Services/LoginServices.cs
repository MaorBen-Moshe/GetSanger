using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class LoginServices : Service
    {
        public LoginServices()
        {
        }

        public async void LoginUser(AppMode? i_Mode = null)
        {
            try
            {
                User user = await FireStoreHelper.GetUser(AuthHelper.GetLoggedInUserId());
                if(user != null)
                {
                    AppManager.Instance.ConnectedUser = user;
                    if (i_Mode != null) // we are here from mode page
                    {
                        chooseModeHelper((AppMode)i_Mode);
                    }
                    else // we are here from login page
                    {
                        if (user.LastUserMode == null)
                        {
                            await new NavigationService().NavigateTo(ShellRoutes.ModePage);
                        }

                        chooseModeHelper((AppMode)user.LastUserMode);
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("User is not available", e);
            }
        }

        public async void SetMode(AppMode i_Mode, Shell i_ChosenShell)
        {
            if (await AuthHelper.IsVerifiedEmail() == false)
            {
                throw new Exception("Please verify your email address to continue!");
            }

            AppManager.Instance.CurrentMode = i_Mode;
            AppManager.Instance.ConnectedUser.LastUserMode = i_Mode;
            await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
            Application.Current.MainPage = i_ChosenShell;
        }

        private void chooseModeHelper(AppMode i_Mode)
        {
            switch (i_Mode)
            {
                case AppMode.Client: SetMode(i_Mode, new UserShell()); break;
                case AppMode.Sanger: SetMode(i_Mode, new SangerShell()); break;
                default: break;
            }
        }

        public override void SetDependencies()
        {
            //
        }
    }
}
