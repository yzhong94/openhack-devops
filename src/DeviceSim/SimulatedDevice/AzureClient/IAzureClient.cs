


using Microsoft.WindowsAzure.MobileServices;

namespace SimulatedDevice.AzureClient
{
    public interface IAzureClient
    {
        IMobileServiceClient Client { get; }
    }
}