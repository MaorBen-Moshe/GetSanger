using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Interfaces
{
    public interface IRunTasks
    {
        Task RunTaskWhileLoading(Task i_InnerTask, ContentPage i_OptionalLoading = null);

        Task<T> RunTaskWhileLoading<T>(Task<T> i_InnerTask, ContentPage i_OptionalLoading = null);
    }
}