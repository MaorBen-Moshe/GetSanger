using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetSanger.Services
{
    public enum eSocialProvider
    {
        Facebook,
        Google,
        Apple,
        Email
    } // email is for the link only

    public class SocialAdapterService : Service, ISocialAdapter
    {
        private ILogin m_LoginService;
        private INavigation m_NavigationService;
        private IPhotoDisplay m_PhotoDisplay;

        public async Task<bool> SocialLogin(eSocialProvider i_Provider)
        {
            SetDependencies();
            bool succeeded = true;
            Dictionary<string, object> details = await AuthHelper.LoginWithProvider(i_Provider);
            bool isFirstLoggedin = await AuthHelper.IsFirstTimeLogIn();
            if (isFirstLoggedin)
            {
                string json = ObjectJsonSerializer.SerializeForPage(getDetails(details));
                string route = $"{ShellRoutes.SignupPersonalDetails}?isFacebookGmail={true}&userJson={json}";
                await m_NavigationService.NavigateTo(route);
            }
            else
            {
                succeeded = await m_LoginService.LoginUser(socialLogin: true);
            }

            return succeeded;
        }

        public async Task SocialLink(eSocialProvider i_Provider) // except eSocialProvider.Email
        {
            SetDependencies();
            Dictionary<string, object> details = await AuthHelper.LinkWithSocialProvider(i_Provider);
            string photoUrl = details.ContainsKey("photoUrl") ? details["photoUrl"] as string : null;
            await m_PhotoDisplay.TryGetPictureFromUri(photoUrl, AppManager.Instance.ConnectedUser);
        }

        private User getDetails(Dictionary<string, object> i_Details)
        {
            string displayName = i_Details.ContainsKey("displayName") ? i_Details["displayName"] as string : null;
            User user = new User
            {
                PersonalDetails = new PersonalDetails
                {
                    NickName = displayName
                },
                Email = i_Details["email"] as string,
                UserId = AuthHelper.GetLoggedInUserId()
            };

            string photoUrl = i_Details.ContainsKey("photoUrl") ? i_Details["photoUrl"] as string : null;
            if (photoUrl != null && user.ProfilePictureUri == null)
            {
                user.ProfilePictureUri = new Uri(photoUrl);
            }

            return user;
        }

        public override void SetDependencies()
        {
            m_LoginService ??= AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            m_NavigationService ??= AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            m_PhotoDisplay ??= AppManager.Instance.Services.GetService(typeof(PhotoDisplayService)) as PhotoDisplayService;
        }
    }
}