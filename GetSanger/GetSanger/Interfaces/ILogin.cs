using GetSanger.Services;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface ILogin
    {
        Task TryAutoLogin();

        Task<bool> LoginUser(eAppMode? i_Mode = null, bool socialLogin = false);
    }
}
