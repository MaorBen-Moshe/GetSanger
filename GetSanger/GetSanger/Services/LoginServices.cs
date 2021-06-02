using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xamarin.Forms;
using GetSanger.ViewModels;

namespace GetSanger.Services
{
    public class LoginServices : Service
    {
        private NavigationService m_NavigationService;
        private RunTasksService m_RunTasks;

        public LoginServices()
        {
        }

        public async void TryAutoLogin()
        {
            SetDependencies();
            if (AuthHelper.IsLoggedIn())
            {
                string userId = AuthHelper.GetLoggedInUserId();
                bool firstTime = await AuthHelper.IsFirstTimeLogIn();
                if (!firstTime)
                {
                    AppManager.Instance.ConnectedUser = await FireStoreHelper.GetUser(userId);
                    AppMode? mode = AppManager.Instance.ConnectedUser.LastUserMode;
                    if (mode == null)
                    {
                        await m_NavigationService.NavigateTo(ShellRoutes.ModePage);
                    }
                    else
                    {
                        AppManager.Instance.CurrentMode = (AppMode)mode;
                        Application.Current.MainPage = AppManager.Instance.GetCurrentShell();
                    }

                    return;
                }
                else
                {
                    string json = ObjectJsonSerializer.SerializeForPage(AppManager.Instance.ConnectedUser);
                    Application.Current.MainPage = new AuthShell();
                    await m_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={true}&userJson={json}");
                }
            }
            else
            {
                Application.Current.MainPage = new AuthShell();
            }
        }

        public async void LoginUser(AppMode? i_Mode = null)
        {
            SetDependencies();

            try
            {
                User user = await m_RunTasks.RunTaskWhileLoading(FireStoreHelper.GetUser(AuthHelper.GetLoggedInUserId()));
                if(user != null)
                {
                    AppManager.Instance.ConnectedUser = user;
                    if (i_Mode != null) // we are here from mode page or from auto login
                    {
                        SetMode((AppMode)i_Mode);
                    }
                    else // we are here from login page
                    {
                        if (user.LastUserMode == null)
                        {
                            await m_NavigationService.NavigateTo(ShellRoutes.ModePage);
                        }
                        else
                        {
                            SetMode();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("User is not available", e);
            }
        }

        public async void SetMode(AppMode? i_Mode = null)
        {
            bool verified = await AuthHelper.IsVerifiedEmail();
            if (verified == false)
            {
                throw new Exception("Please verify your email address to continue!");
            }

            Application.Current.MainPage = AppManager.Instance.GetCurrentShell(i_Mode);
        }

        public override void SetDependencies()
        {
            m_NavigationService ??= AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            m_RunTasks ??= AppManager.Instance.Services.GetService(typeof(RunTasksService)) as RunTasksService;
        }
    }
}
