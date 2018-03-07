using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MyDriving.ServiceObjects;
using Newtonsoft.Json;

namespace MyDriving.POIService.v1
{
    public static class POIService
    {
        [FunctionName("GetAllPOIs")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string tripId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "tripId", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            tripId = tripId ?? data?.tripId;

            List<POI> POIList;

            using (var context = new MyDrivingContext())
            {
                POIList = context.POIs.Where(x => x.TripId == tripId).ToList<POI>();
            }

            var json = JsonConvert.SerializeObject(POIList);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public static string HealthCheck() => "ok";
    }
}
