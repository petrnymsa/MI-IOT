using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
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
        public DbSet<Settings> Settings { get; set; }

        public DbSet<HeartBeatInfo> HeartBeatInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<TemperatureSensorSnapshot>().ToTable("TemperatureSensorSnapshot");
            b.Entity<HumiditySensorSnapshot>().ToTable("HumiditySensorSnapshot");
            b.Entity<Settings>().ToTable("Settings");
        }
    }

    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ServerDbContext>
    {
        public ServerDbContext CreateDbContext(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(currentDir);
            configBuilder.AddJsonFile("appsettings.json");
            var config = configBuilder.Build();
           var connectionString = config.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<ServerDbContext>();
            builder.UseSqlServer(connectionString);
            var context = new ServerDbContext(builder.Options);
            return context;
        }
    }
}
