using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Data
{
    public class InputDataParserResult
    {
        public TemperatureSensorSnapshot TemperatureSensor { get; set; }
        public HumiditySensorSnapshot HumiditySensor { get; set; }
    }
    public interface IInputDataParser
    {
        InputDataParserResult Parse(string raw, DateTime dateRecord);
    }

    public class InputDataParser : IInputDataParser
    {

        public InputDataParser()
        {
        }

        public InputDataParserResult Parse(string raw, DateTime dateRecord)
        {
            var chunks = raw.Split(';',StringSplitOptions.RemoveEmptyEntries);
            if (chunks.Length == 2)
                return ParseOnlyTemperatureSensor(chunks, dateRecord);
            else if (chunks.Length == 3)
                return ParseBoth(chunks, dateRecord);

            throw new FormatException($"Raw input {raw} has insufficient number of splitted chunks");
        }

        private InputDataParserResult ParseOnlyTemperatureSensor(string[] chunks, DateTime date)
        {
            var tempSensor = CreateTemperatureSensor(chunks[0], chunks[1], date);
            return new InputDataParserResult()
            {
                TemperatureSensor = tempSensor,
                HumiditySensor = null
            };
        }

        private InputDataParserResult ParseBoth(string[] chunks, DateTime date)
        {
            var tempSensor = CreateTemperatureSensor(chunks[0], chunks[1], date);
            var humSensor = CreateHumiditySensor(chunks[2], date);
            return new InputDataParserResult()
            {
                TemperatureSensor = tempSensor,
                HumiditySensor = humSensor
            };
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
