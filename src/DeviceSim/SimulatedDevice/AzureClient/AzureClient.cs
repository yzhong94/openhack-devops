


using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using System.Threading.Tasks;
using SimulatedDevice.Utils;
using Newtonsoft.Json.Linq;
using System;

namespace SimulatedDevice.AzureClient
{
    public class AzureClient : IAzureClient
    {
        const string DefaultMobileServiceUrl = "https://mydriving-vpwupcazgfita.azurewebsites.net";
        static IMobileServiceClient client;

        public IMobileServiceClient Client => client ?? (client = CreateClient());

        IMobileServiceClient CreateClient()
        {
            client = new MobileServiceClient(DefaultMobileServiceUrl, new AuthHandler())
            {
                SerializerSettings = new MobileServiceJsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    CamelCasePropertyNames = true
                }
            };
            return client;
        }

        public static async Task CheckIsAuthTokenValid()
        {
            //Check if the access token is valid by sending a general request to mobile service
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            try
            {
                await client.InvokeApiAsync("/.auth/me", HttpMethod.Get, null);
            }
            catch { } //Eat any exceptions
        }
    }
}