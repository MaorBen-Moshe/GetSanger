using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IAuth
    {
        Task<string> GetIdToken();
        Task<string> GetUserId();
    }
}
