﻿using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Models;
using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using GetSanger.Views.popups;
using GetSanger.Interfaces;
using GetSanger.Extensions;

namespace GetSanger.Services
{
    public class LoginServices : Service, ILogin
    {
        private Interfaces.INavigation m_NavigationService;
        private ILocation m_Location;
        private IUiPush m_PushServices;

        public LoginServices()
        {
        }

        public async Task TryAutoLogin()
        {
            SetDependencies();
            try
            {
                if (await AuthHelper.IsLoggedIn())
                {
                    string userId = AuthHelper.GetLoggedInUserId();
                    bool firstTime = await AuthHelper.IsFirstTimeLogIn();
                    if (!firstTime)
                    {
                        AppManager.Instance.ConnectedUser = await FireStoreHelper.GetUser(userId);
                        await Task.Run(async () =>
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                AppManager.Instance.ConnectedUser.UserLocation = await m_Location.GetCurrentLocation();
                                await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                                return;
                            });
                             
                            bool isRegistrationTokenChanged = await m_PushServices.IsRegistrationTokenChanged();
                            if (isRegistrationTokenChanged)
                            {
                                 AppManager.Instance.ConnectedUser.RegistrationToken = await m_PushServices.GetRegistrationToken();
                            }


                            await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                        });

                        eAppMode? mode = AppManager.Instance.ConnectedUser.LastUserMode;
                        if (mode == null)
                        {
                            await Task.Delay(1500);
                            Application.Current.MainPage = new AuthShell();
                            await PopupNavigation.Instance.PushAsync(new ModePage());
                        }
                        else
                        {
                            AppManager.Instance.CurrentMode = (eAppMode)mode;
                            Application.Current.MainPage = AppManager.Instance.GetCurrentShell();
                            await PushServices.HandleMessageReceived(null, null, PushServices.BackgroundPushData);
                        }
                    }
                    else
                    {
                        AppManager.Instance.ConnectedUser = new User { UserId = userId };
                        string json = null;
                        await Task.Delay(1500);
                        Application.Current.MainPage = new AuthShell();
                        await m_NavigationService.NavigateTo(ShellRoutes.SignupPersonalDetails + $"?isFacebookGmail={false}&userJson={json}");
                    }
                }
                else
                {
                    await Task.Delay(1500);
                    Application.Current.MainPage = new AuthShell();
                }
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(LoginServices)}:TryAutoLogin", "Error", e.Message);
            }
        }

        public async Task<bool> LoginUser(eAppMode? i_Mode = null, bool socialLogin = false)
        {
            SetDependencies();
            try
            {
                bool firstTime = await AuthHelper.IsFirstTimeLogIn();
                bool verified = false;
                if (firstTime == false)
                {
                    User user = await FireStoreHelper.GetUser(AuthHelper.GetLoggedInUserId());
                    verified = await AuthHelper.IsVerifiedEmail();
                    AppManager.Instance.ConnectedUser = user;
                    AppManager.Instance.ConnectedUser.UserLocation = await m_Location.GetCurrentLocation();
                    bool isRegistrationTokenChanged = await m_PushServices.IsRegistrationTokenChanged();
                    if (isRegistrationTokenChanged)
                    {
                        AppManager.Instance.ConnectedUser.RegistrationToken = await m_PushServices.GetRegistrationToken();
                    }

                    await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);

                    if (verified)
                    {
                        await m_PushServices.SubscribeUser(AppManager.Instance.ConnectedUser.UserId);

                        if (i_Mode != null) // we are here from mode page or from auto login
                        {
                            setMode((eAppMode) i_Mode);
                        }
                        else // we are here from login page
                        {
                            if (user.LastUserMode == null)
                            {
                                await PopupNavigation.Instance.PushAsync(new ModePage());
                            }
                            else
                            {
                                setMode();
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

        private void setMode(eAppMode? i_Mode = null)
        {
            Application.Current.MainPage = AppManager.Instance.GetCurrentShell(i_Mode);
        }

        public override void SetDependencies()
        {
            m_NavigationService ??= AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            m_Location ??= AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            m_PushServices ??= AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
        }
    }
}