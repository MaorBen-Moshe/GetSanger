using GetSanger.AppShell;
using GetSanger.Constants;
using GetSanger.Models;
using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using GetSanger.Views.popups;
using GetSanger.Interfaces;

namespace GetSanger.Services
{
    public class LoginServices : Service, ILogin
    {
        private Interfaces.INavigation m_NavigationService;
        private ILocation m_Location;
        private ITrip m_Trip;
        private IUiPush m_PushServices;

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
                    AppManager.Instance.ConnectedUser.UserLocation = await m_Location.GetCurrentLocation();

                    bool isRegistrationTokenChanged = await m_PushServices.IsRegistrationTokenChanged();
                    if (isRegistrationTokenChanged)
                    {
                        AppManager.Instance.ConnectedUser.RegistrationToken = await m_PushServices.GetRegistrationToken();
                    }

                    await FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                    eAppMode? mode = AppManager.Instance.ConnectedUser.LastUserMode;
                    if (mode == null)
                    {
                        await Task.Delay(1500);
                        Application.Current.MainPage = new AuthShell();
                        await PopupNavigation.Instance.PushAsync(new ModePage());
                    }
                    else
                    {
                        AppManager.Instance.CurrentMode = (eAppMode) mode;
                        Application.Current.MainPage = AppManager.Instance.GetCurrentShell();
                        await PushServices.HandleMessageReceived(null, null, PushServices.BackgroundPushData);
                        bool shared = await m_Trip.TryShareSangerLoaction();
                        if (shared)
                        {
                            m_Trip.StartTripThread();
                        }
                    }
                }
                else
                {
                    AppManager.Instance.ConnectedUser = new User {UserId = userId};
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
                            bool shared = await m_Trip.TryShareSangerLoaction();
                            if (shared)
                            {
                                m_Trip.StartTripThread();
                            }
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
                                bool shared = await m_Trip.TryShareSangerLoaction();
                                if (shared)
                                {
                                    m_Trip.StartTripThread();
                                }
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
            m_Trip ??= AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
        }
    }
}