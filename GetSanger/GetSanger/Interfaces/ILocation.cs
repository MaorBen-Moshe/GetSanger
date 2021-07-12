using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GetSanger.Interfaces
{
    public interface ILocation
    {
        CancellationTokenSource Cts { get; set; }

        Task<Location> GetCurrentLocation();

        Task<PermissionStatus> IsLocationGrantedAndAskFor();

        Task<bool> IsLocationGranted();

        Task<Placemark> GetPickedLocation(Location i_Location);

        void Cancelation();
    }
}
