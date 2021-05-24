using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public enum SocialProvider { Facebook, Google, Apple }
    public class SocialAdapterService : Service
    {
        private LoginServices m_LoginService;
        private NavigationService m_NavigationService;

        public async void SocialLogin(SocialProvider i_Provider)
        {
            if(m_LoginService == null || m_NavigationService == null)
            {
                SetDependencies();
            }

            Dictionary<string, object> details = null;
            IPopupService service = DependencyService.Get<IPopupService>();
            service.ShowPopupgPage();
            switch (i_Provider)
            {
                case SocialProvider.Facebook: details = await AuthHelper.LoginViaFacebook(); break;
                case SocialProvider.Google: details = await AuthHelper.LoginViaGoogle(); break;
                case SocialProvider.Apple: details = await AuthHelper.LoginViaApple(); break;
            }

            bool isFirstLoggedin = await AuthHelper.IsFirstTimeLogIn();
            service.HidePopupPage();
            if (isFirstLoggedin)
            {
                AppManager.Instance.SignUpVM.UserJson = JsonSerializer.Serialize(getDetails(details));
                string route = $"{ShellRoutes.SignupPersonalDetails}?isFacebookGmail={true}";
                await m_NavigationService.NavigateTo(route);
            }
            else
            {
                m_LoginService.LoginUser();
            }
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
