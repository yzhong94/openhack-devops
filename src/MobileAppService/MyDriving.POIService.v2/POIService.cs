
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using MyDriving.ServiceObjects;
using System;

namespace MyDriving.POIService.v2
{
    public static class POIService
    {
        [FunctionName("GetAllPOIs")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            IConfiguration funcConfiguration;

            log.Info("C# HTTP trigger function processed a request.");

            string tripId = req.Query["tripId"];

            //var sqlConn = new SqlConnection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            funcConfiguration = builder.Build();

            var connectionString = funcConfiguration["ConnectionStrings:myDrivingDB"];

            using(var sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();

                string query = $"SELECT Id, Deleted, Latitude, Longitude, POIType, Timestamp, TripId FROM POIs WHERE TripId = '{tripId}'";

                var sqlCommand = new SqlCommand(query, sqlConn);

                var rows = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                if(!rows.HasRows)
                    return new BadRequestObjectResult("There are no POIs for this Trip.");

                List<POI> poiList = new List<POI>();

                while (rows.Read())
                {
                    poiList.Add(new POI
                    {
                        Id = rows["Id"].ToString(),
                        Deleted = bool.Parse(rows["Deleted"].ToString()),
                        Latitude = double.Parse(rows["Latitude"].ToString()),
                        Longitude = double.Parse(rows["Longitude"].ToString()),
                        POIType = rows["POIType"].ToString().Equals("HardAcceleration") ? POIType.HardAcceleration : POIType.HardBrake,
                        Timestamp = DateTime.Parse(rows["Timestamp"].ToString()),
                        TripId = rows["TripId"].ToString()
                    });
                }

                var poisSerialized = JsonConvert.SerializeObject(poiList);

                return (ActionResult)new OkObjectResult(poisSerialized);
            }

            return connectionString != null
                ? (ActionResult)new OkObjectResult($"ConnectionString: {connectionString}")
                : new BadRequestObjectResult("Could not load configuration file.");
        }
    }
}
