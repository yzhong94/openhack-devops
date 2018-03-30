using SimulatedDevice.DataObjects;
using SimulatedDevice.DataStore.Abstractions;
using SimulatedDevice.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedDevice.DataStore.Mock.Stores
{
    public class TripPointStore : BaseStore<TripPoint>, ITripPointStore
    {
        ITripStore tripStore;

        public TripPointStore()
        {
            tripStore = ServiceLocator.Instance.Resolve<ITripStore>();
        }
        public async Task<IEnumerable<TripPoint>> GetPointsForTripAsync(string id)
        {
            return (await tripStore.GetItemAsync(id)).Points;
        }
    }
}
