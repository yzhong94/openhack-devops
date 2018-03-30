using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimulatedDevice.Services;

namespace SimulatedDevice.Interfaces
{
    public interface IGeocoder
    {
        Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address);
        Task<IEnumerable<string>> GetAddressesForPositionAsync(Position position);
    }


}
