
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Linq;
using MyDriving.ServiceObjects;

namespace MyDriving.POIService
{
    public static class POIService
    {
        [FunctionName("GetAllPOIs")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string tripId = req.Query["tripId"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            tripId = tripId?? data?.tripId;

            var context = new MyDrivingContext();

            context.POIs.Select(x => x.Id == tripId).ToList();

            return tripId != null
                ? (ActionResult)new OkObjectResult($"Hello, {tripId}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
