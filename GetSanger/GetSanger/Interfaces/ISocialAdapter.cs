using GetSanger.Services;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface ISocialAdapter
    {
        Task<bool> SocialLogin(eSocialProvider i_Provider);

        Task SocialLink(eSocialProvider i_Provider);
    }
}