using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Data
{
    public class TemperatureSensorDataParser : IInputDataParser<TemperatureSensorSnapshot>
    {
        public TemperatureSensorSnapshot Parse(string raw, DateTime date)
        {
            var chunks = raw.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (chunks.Length != 2)
                throw new FormatException($"Raw input {raw} has insufficient number of splitted chunks");

            return CreateTemperatureSensor(chunks[0], chunks[1], date);
        }

        private TemperatureSensorSnapshot CreateTemperatureSensor(string temperature, string humidity, DateTime date)
        {
            try
            {
                return new TemperatureSensorSnapshot()
                {
                    Temperature = float.Parse(temperature),
                    Humidity = float.Parse(humidity),
                    Date = date
                };
            }
            catch (FormatException)
            {
                throw new ArgumentException($"TemperatureSensor data has incorrect format: {temperature};{humidity}");
            }
        }
    }
}
