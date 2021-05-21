using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IDialService
    {
        string PhoneNumber { get; set; }

        string Message { get; set; }

        Task<bool> SendWhatsapp();

        void SendDefAppMsg();

        void Call();

       bool IsValidPhone(string i_Phone);
    }
}
