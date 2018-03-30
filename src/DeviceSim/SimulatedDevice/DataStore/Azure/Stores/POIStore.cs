


using SimulatedDevice.DataObjects;
using SimulatedDevice.DataStore.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SimulatedDevice.DataStore.Azure.Stores
{
    public class POIStore : BaseStore<POI>, IPOIStore
    {
        public async Task<IEnumerable<POI>> GetItemsAsync(string tripId)
        {
            //Always force refresh
            await InitializeStoreAsync();
            await SyncAsync();
            return await Table.CreateQuery().Where(p => p.TripId == tripId).ToEnumerableAsync();
        }
    }
}