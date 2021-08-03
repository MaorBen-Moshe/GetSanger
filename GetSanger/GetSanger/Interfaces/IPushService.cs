using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IPushService
    {
        Task<string> GetRegistrationToken();
    }
}