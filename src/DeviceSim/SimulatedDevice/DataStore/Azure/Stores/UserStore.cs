


using SimulatedDevice.DataObjects;
using SimulatedDevice.DataStore.Abstractions;
using System.Linq;

namespace SimulatedDevice.DataStore.Azure.Stores
{
    public class UserStore : BaseStore<UserProfile>, IUserStore
    {
        public override string Identifier => "User";

        public override async System.Threading.Tasks.Task<UserProfile> GetItemAsync(string id)
        {
            var users = await base.GetItemsAsync(0, 10, true);
            return users.FirstOrDefault(s => s.UserId == id);
        }
    }
}