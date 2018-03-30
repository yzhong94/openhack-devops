using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimulatedDevice.Interfaces
{
    public interface IHubIOT
    {
        void Initialize(string connectionStr);
        Task SendEvents(IEnumerable<String> blobs);
        Task SendEvent(string blob);
    }
}
