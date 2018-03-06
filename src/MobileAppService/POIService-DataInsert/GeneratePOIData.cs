using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using System.Data.SqlClient;

using MyDriving.ServiceObjects;

namespace MyDriving.POIService.v1
{
    public static class GeneratePOIData
    {
        [FunctionName("GeneratePOIData")]
        public static void Run([TimerTrigger("*/1 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            //log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            string TripId = Guid.NewGuid().ToString();

            string _connectionString = "Server=tcp:mydrivingdbserver-fxw5u47lzepqy.database.windows.net,1433;Initial Catalog=mydrivingDB;Persist Security Info=False;User ID=YourUserName;Password=@windowsPhone10;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(GetSQLCommand(TripId), conn))
                {
                    log.Info($"{cmd.ExecuteNonQuery()} rows were updated");
                }
            }
        }

        private static string GetSQLCommand(string TripId)
        {
            return "DECLARE @datetimeoffset datetimeoffset(7) = '" + DateTime.Now.ToString() + "'" +
                "INSERT INTO [dbo].[POIs]" +
                       "([TripId]" +
                       ",[Latitude]" +
                       ",[Longitude]" +
                       ",[POIType]" +
                       ",[RecordedTimeStamp]" +
                       ",[CreatedAt]" +
                       ",[UpdatedAt]" +
                       ",[Deleted]" +
                       ",[Timestamp])" +
                         "VALUES('" + TripId.ToString() + "'" +
                         ", " + GetLatitude(516400146, 630304598) +
                          "," + GetLongitude(224464416, 341194152) +
                         ", 1" +
                         ", '" + DateTime.Now.ToString() + "'" +
                         ", '" + DateTime.Now.ToString() + "'" +
                         ", @datetimeoffset" +
                         ", 0" +
                         ", '" + DateTime.Now.ToString() + "')";
        }

        private static int GetLatitude(int From, int To)
        {
            return GenerateRandom(From, To);
        }

        private static double GetLongitude(int From, int To)
        {
            return GenerateRandom(From, To);
        }

        private static int GenerateRandom(int From, int To)
        {
            Random rng = new Random();
            return rng.Next(From, To);
        }

    }
}
