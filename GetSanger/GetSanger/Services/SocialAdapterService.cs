using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public enum SocialProvider { Facebook, Gmail, Apple }
    public class SocialAdapterService : Service
    {
        private LoginServices m_LoginService;
        private NavigationService m_NavigationService;

        public async void SocialLogin(SocialProvider i_Provider)
        {
            Dictionary<string, object> details = null;
            IPopupService service = DependencyService.Get<IPopupService>();
            service.ShowPopupgPage();
            switch (i_Provider)
            {
                case SocialProvider.Facebook: details = await AuthHelper.LoginViaFacebook(); break;
                case SocialProvider.Gmail: details = await AuthHelper.LoginViaGoogle(); break;
                case SocialProvider.Apple: details = await AuthHelper.LoginViaApple(); break;
            }

            bool isFirstLoggedin = await AuthHelper.IsFirstTimeLogIn();
            service.HidePopupPage();
            if (isFirstLoggedin)
            {
                setSerializedUser(i_Provider, details);
                string route = $"{ShellRoutes.SignupPersonalDetails}?isFacebookGmail={true}";
                await ((NavigationService)AppManager.Instance.Services.GetService(typeof(NavigationService))).NavigateTo(route);
            }
            else
            {
                m_LoginService.LoginUser();
            }
        }

        private void setSerializedUser(SocialProvider i_Provider, Dictionary<string, object> i_Details)
        {
            string json = null;
            switch (i_Provider)
            {
                case SocialProvider.Facebook: json = JsonSerializer.Serialize(getFacebookDetails(i_Details)); break;
                case SocialProvider.Gmail: json = JsonSerializer.Serialize(getGmailDetails(i_Details)); break;
                case SocialProvider.Apple: json = JsonSerializer.Serialize(getAppleDetails(i_Details)); break;
            }

            AppManager.Instance.SignUpVM.UserJson = json;
        }

        private User getGmailDetails(Dictionary<string, object> i_Details)
        {
            User user = new User
            {
                PersonalDetails = new PersonalDetails
                {
                    NickName = i_Details["displayName"] as string
                },
                Email = i_Details["email"] as string,
            };

            if (i_Details["photoUrl"] != null)
            {
                user.ProfilePictureUri = new Uri(i_Details["photoUrl"] as string);
            }

            return user;
        }

        private User getFacebookDetails(Dictionary<string, object> i_Details)
        {
            throw new NotImplementedException();
        }

        private User getAppleDetails(Dictionary<string, object> i_Details)
        {
            throw new NotImplementedException();
        }

        public override void SetDependencies()
        {
            m_LoginService = ((LoginServices)AppManager.Instance.Services.GetService(typeof(LoginServices)));
            m_NavigationService = ((NavigationService)AppManager.Instance.Services.GetService(typeof(NavigationService)));
        }
    }
}
