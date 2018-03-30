


using SimulatedDevice.DataObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimulatedDevice.DataStore.Abstractions
{
    public interface IPOIStore : IBaseStore<POI>
    {
        Task<IEnumerable<POI>> GetItemsAsync(string tripId);
    }
}