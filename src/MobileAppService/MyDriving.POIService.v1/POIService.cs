using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MyDriving.ServiceObjects;

namespace MyDriving.POIService.v1
{
    public static class POIService
    {
        [FunctionName("GetAllPOIs")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
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

            using(var context = new MyDrivingContext())
            {
                context.POIs.Select(x => x.Id == tripId).ToList();
            }

            return tripId == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + tripId);
        }
    }
}
