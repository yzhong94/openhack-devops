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
            string TripId = Guid.NewGuid().ToString();

            using (var context = new MyDrivingContext())
            {
                for (int i = 0; i < 5; i++)
                {
                    context.POIs.Add(
                        new POI
                        {
                            TripId = TripId,
                            Latitude = GetLatitude(516400146, 630304598),
                            Longitude = GetLongitude(224464416, 341194152),
                            POIType = POIType.HardAcceleration,
                            Timestamp = DateTime.Now,
                            Deleted = false
                        });

                    context.SaveChanges();

                    log.Info($"TripId {TripId} saved");

                }
               
            }
        }


        //private static string GetSQLCommand(string TripId)
        //{
        //    return "DECLARE @datetimeoffset datetimeoffset(7) = '" + DateTime.Now.ToString() + "'" +
        //        "INSERT INTO [dbo].[POIs]" +
        //               "([TripId]" +
        //               ",[Latitude]" +
        //               ",[Longitude]" +
        //               ",[POIType]" +
        //               ",[RecordedTimeStamp]" +
        //               ",[CreatedAt]" +
        //               ",[UpdatedAt]" +
        //               ",[Deleted]" +
        //               ",[Timestamp])" +
        //                 "VALUES('" + TripId.ToString() + "'" +
        //                 ", " + GetLatitude(516400146, 630304598) +
        //                  "," + GetLongitude(224464416, 341194152) +
        //                 ", 1" +
        //                 ", '" + DateTime.Now.ToString() + "'" +
        //                 ", '" + DateTime.Now.ToString() + "'" +
        //                 ", @datetimeoffset" +
        //                 ", 0" +
        //                 ", '" + DateTime.Now.ToString() + "')";
        //}

        #region CORDINATES
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
        #endregion
    }
}
