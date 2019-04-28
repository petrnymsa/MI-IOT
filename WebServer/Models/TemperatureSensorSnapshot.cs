using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class TemperatureSensorSnapshot
    {
        public int Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }

        public DateTime Date { get; set; }

        public override string ToString() => $"{Id};{Temperature};{Humidity};{Date}";
    }
}
