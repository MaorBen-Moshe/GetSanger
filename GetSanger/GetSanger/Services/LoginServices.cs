﻿using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using GetSanger.ViewModels;
using GetSanger.Exceptions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using Xamarin.Essentials;

namespace GetSanger.Services
{
    public class LoginServices : Service
    {
        private NavigationService m_NavigationService;
        private RunTasksService m_RunTasks;
        private LocationService m_Location;

        public LoginServices()
        {
        }

        public async Task TryAutoLogin()
        {
            SetDependencies();
            if (AuthHelper.IsLoggedIn())
            {
                string userId = AuthHelper.GetLoggedInUserId();
                bool firstTime = await AuthHelper.IsFirstTimeLogIn();
                if (!firstTime)
                {
                    AppManager.Instance.ConnectedUser = await FireStoreHelper.GetUser(userId);
                    AppManager.Instance.ConnectedUser.UserLocation ??= await m_Location.GetCurrentLocation();
                    await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                    AppMode? mode = AppManager.Instance.ConnectedUser.LastUserMode;
                    if (mode == null)
                    {
                        await m_NavigationService.NavigateTo(ShellRoutes.ModePage);
                    }
                    else
                    {
                        AppManager.Instance.CurrentMode = (AppMode) mode;
                        Application.Current.MainPage = AppManager.Instance.GetCurrentShell();
                    }

                    return;
                }
                else
                {
                    try
                    {
                        AppManager.Instance.ConnectedUser = new User {UserId = userId};
                        //string json = ObjectJsonSerializer.SerializeForPage(AppManager.Instance.ConnectedUser);
                        string json = null;
                        await Task.Delay(1500);
                        Application.Current.MainPage = new AuthShell();
                        await m_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={false}&userJson={json}");
                    }
                    catch (Exception e)
                    {
                        string message = e.Message;
                        Console.WriteLine(message);
                    }
                }
            }
            else
            {
                await Task.Delay(1500);
                Application.Current.MainPage = new AuthShell();
            }
        }

        public async Task<bool> LoginUser(AppMode? i_Mode = null, bool socialLogin = false)
        {
            SetDependencies();

            try
            {
                bool firstTime = await AuthHelper.IsFirstTimeLogIn();
                bool verified = false;
                if (firstTime == false)
                {
                    User user = await m_RunTasks.RunTaskWhileLoading(FireStoreHelper.GetUser(AuthHelper.GetLoggedInUserId()));
                    verified = await AuthHelper.IsVerifiedEmail();
                    AppManager.Instance.ConnectedUser = user;
                    AppManager.Instance.ConnectedUser.UserLocation ??= await m_Location.GetCurrentLocation();
                    await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                    if (verified)
                    {
                        if (i_Mode != null) // we are here from mode page or from auto login
                        {
                            SetMode((AppMode) i_Mode);
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
                else
                {
                    await m_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={socialLogin}");
                    return true;
                }

                return verified;
            }
            catch (Exception e)
            {
                throw new Exception("User is not available", e);
            }
        }

        public void SetMode(AppMode? i_Mode = null)
        {
            Application.Current.MainPage = AppManager.Instance.GetCurrentShell(i_Mode);
        }

        public override void SetDependencies()
        {
            m_NavigationService ??= AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            m_RunTasks ??= AppManager.Instance.Services.GetService(typeof(RunTasksService)) as RunTasksService;
            m_Location ??= AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
        }
    }
}