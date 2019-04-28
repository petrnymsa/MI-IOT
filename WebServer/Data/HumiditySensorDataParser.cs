using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Data
{
    public class HumiditySensorDataParser : IInputDataParser<HumiditySensorSnapshot>
    {
        public HumiditySensorSnapshot Parse(string raw, DateTime date)
        {
            var chunks = raw.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (chunks.Length != 1)
                throw new FormatException($"Raw input {raw} has insufficient number of splitted chunks");

            return CreateHumiditySensor(chunks[0], date);
        }

        private HumiditySensorSnapshot CreateHumiditySensor(string humidity, DateTime date)
        {
            try
            {
                return new HumiditySensorSnapshot()
                {
                    Humidity = float.Parse(humidity),
                    Date = date
                };
            }
            catch (FormatException)
            {
                throw new ArgumentException($"HumiditySensor data has incorrect format: {humidity}");

            }
        }
    }
}
