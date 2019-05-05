using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ServerDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.TemperatureSensorSnapshots.Any())
            {
                return;   // DB has been seeded
            }
            var date = DateTime.Now;
            var tempSnapshots = new TemperatureSensorSnapshot[]
            {
                new TemperatureSensorSnapshot{Temperature = 22.5f, Humidity = 0.3f, Date = date },
                new TemperatureSensorSnapshot{Temperature = 23.2f, Humidity = 0.1f, Date = date.AddDays(1) },
                new TemperatureSensorSnapshot{Temperature = 21.8f, Humidity = 0.6f, Date = date.AddDays(2) },
                new TemperatureSensorSnapshot{Temperature = 25.9f, Humidity = 0.2f, Date = date.AddDays(3) },
            };

            context.TemperatureSensorSnapshots.AddRange(tempSnapshots);
            context.SaveChanges();

            var humSnapshots = new HumiditySensorSnapshot[]
            {
                new HumiditySensorSnapshot{Humidity = 0.8f, Date = date},
                new HumiditySensorSnapshot{Humidity = 0.76f, Date = date.AddHours(1)},
                new HumiditySensorSnapshot{Humidity = 0.6f, Date = date.AddHours(2)},
                new HumiditySensorSnapshot{Humidity = 0.45f, Date = date.AddHours(3)},
            };

            context.HumiditySensorSnapshots.AddRange(humSnapshots);
            context.SaveChanges();

            context.Settings.Add(new Settings()
            {
                DHT11Interval = Settings.DefaultDHT11Interval,
                EsesInterval = Settings.DefaultEsesInterval
            });

            context.SaveChanges();

        }
    }
}
