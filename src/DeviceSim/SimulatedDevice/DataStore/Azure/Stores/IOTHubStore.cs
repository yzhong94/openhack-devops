


using SimulatedDevice.DataObjects;
using SimulatedDevice.DataStore.Abstractions;
using System.Threading.Tasks;

namespace SimulatedDevice.DataStore.Azure.Stores
{
    public class IOTHubStore : BaseStore<IOTHubData>, IHubIOTStore
    {
        public override string Identifier => "IOTHub";

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        object locker = new object();
        public override Task<bool> InsertAsync(IOTHubData item)
        {
            lock(locker)
            {
                return base.InsertAsync(item);
            }
        }
    }
}