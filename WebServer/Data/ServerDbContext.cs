using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Data
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options):base(options)
        {

        }

        public DbSet<TemperatureSensorSnapshot> TemperatureSensorSnapshots { get; set; }
        public DbSet<HumiditySensorSnapshot> HumiditySensorSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<TemperatureSensorSnapshot>().ToTable("TemperatureSensorSnapshot");
            b.Entity<HumiditySensorSnapshot>().ToTable("HumiditySensorSnapshot");
        }
    }
}
