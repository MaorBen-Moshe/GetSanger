using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface INavigation
    {
        Task NavigateTo(string i_ViewPath);
    }
}
