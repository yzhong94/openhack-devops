using SimulatedDevice.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedDevice.DataStore.Abstractions
{
    public interface ITripPointStore : IBaseStore<TripPoint>
    {
        Task<IEnumerable<TripPoint>> GetPointsForTripAsync(string id);
    }
}
