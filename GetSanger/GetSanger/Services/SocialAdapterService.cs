using GetSanger.Constants;
using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public enum eSocialProvider { Facebook, Google, Apple, Email } // email is for the link only

    public class SocialAdapterService : Service
    {
        private LoginServices m_LoginService;
        private NavigationService m_NavigationService;
        private StorageHelper m_StorageHelper;

        public async Task<bool> SocialLogin(eSocialProvider i_Provider)
        {
            SetDependencies();
            bool succeeded = true;
            Dictionary<string, object> details = await AuthHelper.LoginWithProvider(i_Provider);
            bool isFirstLoggedin = await AuthHelper.IsFirstTimeLogIn();
            if (isFirstLoggedin)
            {
                string json = ObjectJsonSerializer.SerializeForPage(await getDetails(details));
                string route = $"{ShellRoutes.SignupPersonalDetails}?isFacebookGmail={true}&userJson={json}";
                await m_NavigationService.NavigateTo(route);
            }
            else
            {
                succeeded = await m_LoginService.LoginUser(socialLogin:true);
            }

            return succeeded;
        }

        private async Task<User> getDetails(Dictionary<string, object> i_Details)
        {
            User user = new User
            {
                PersonalDetails = new PersonalDetails
                {
                    NickName = i_Details["displayName"] as string
                },
                Email = i_Details["email"] as string,
                UserId = AuthHelper.GetLoggedInUserId()
            };

            if (i_Details["photoUrl"] != null)
            {
                using var client = new WebClient();
                var content = client.DownloadData(i_Details["photoUrl"] as string);
                using var stream = new MemoryStream(content);
                var destStream = new MemoryStream();
                await stream.CopyToAsync(destStream);
                m_StorageHelper.SetUserProfileImage(user, destStream);
            }

            return user;
        }

        public override void SetDependencies()
        {
            m_LoginService ??= AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            m_NavigationService ??= AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            m_StorageHelper ??= AppManager.Instance.Services.GetService(typeof(StorageHelper)) as StorageHelper;
        }
    }
}
