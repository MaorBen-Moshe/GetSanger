using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace GetSanger.Interfaces
{
    public interface IAddressInfo
    {
        string Location { get; }
        MapSpan MapSpan { get; }
        ObservableCollection<Pin> Pins { get; }
    }
}
