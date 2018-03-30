


using SimulatedDevice.DataObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimulatedDevice.DataStore.Abstractions
{
    public interface IPhotoStore : IBaseStore<Photo>
    {
        Task<IEnumerable<Photo>> GetTripPhotos(string tripId);
    }
}