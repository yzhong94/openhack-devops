


using SimulatedDevice.DataObjects;
using SimulatedDevice.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimulatedDevice.DataStore.Azure.Stores
{
    public class PhotoStore : BaseStore<Photo>, IPhotoStore
    {
        public override string Identifier => "Photo";

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Photo>> GetTripPhotos(string tripId)
        {
            return Table.Where(s => s.TripId == tripId).ToEnumerableAsync();
        }
    }
}