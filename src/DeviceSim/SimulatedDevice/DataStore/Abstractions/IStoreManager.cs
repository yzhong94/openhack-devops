


using System.Threading.Tasks;

namespace SimulatedDevice.DataStore.Abstractions
{
    public interface IStoreManager
    {
        bool IsInitialized { get; }
        ITripStore TripStore { get; }
        IPhotoStore PhotoStore { get; }
        IUserStore UserStore { get; }
        IHubIOTStore IOTHubStore { get; }
        IPOIStore POIStore { get; }
        ITripPointStore TripPointStore { get; }
        Task<bool> SyncAllAsync(bool syncUserSpecific);
        Task DropEverythingAsync();
        Task InitializeAsync();
    }
}