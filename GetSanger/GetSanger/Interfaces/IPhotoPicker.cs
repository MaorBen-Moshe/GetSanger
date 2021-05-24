using System;
using System.IO;
using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface IPhotoPicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
