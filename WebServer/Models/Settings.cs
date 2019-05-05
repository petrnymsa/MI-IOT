using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class Settings
    {
        public const int DefaultDHT11Interval = 5;
        public const int DefaultEsesInterval = 5;

        public int Id { get; set; }

        public int DHT11Interval { get; set; }
        public int EsesInterval { get; set; }

        public override string ToString()
        {
            return $"{Id};{DHT11Interval};{EsesInterval}";
        }
    }
}
