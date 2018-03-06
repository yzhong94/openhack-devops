using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text;
using System.Configuration;

namespace MyDriving.ServiceObjects
{
    public class MyDrivingContext : DbContext
    {
        public DbSet<POI> POIs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["myDrivingDb"].ToString();
            var connectionString = "Server=tcp:mydrivingdbserver-fxw5u47lzepqy.database.windows.net,1433;Initial Catalog=mydrivingDB;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
