using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MyDriving.ServiceObjects
{
    public class MyDrivingContext : DbContext
    {
        public DbSet<POI> POIs { get; set; }
    }
}
