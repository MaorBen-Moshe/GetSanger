using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IAuth
    {
        Task<string> GetIdToken();
        string GetUserId();
        void SignOut();
        bool IsLoggedIn();
        Task SignInWithCustomToken(string i_Token);
        bool IsAnonymousUser();
        Task SignInAnonymouslyAsync();
        void LoginViaFacebook();
        string GetFacebookAccessToken();
    }
}