using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using WebServer.Data;

namespace WebServer.UnitTests
{
    [TestFixture]
    public class InputDataParserTest
    {
        [Test]
        public void Parse_OnlyTempSensor_ShouldReturnCorrectly()
        {
            var parser = new TemperatureSensorDataParser();
            var input = "18.1;0.5";
            var result = parser.Parse(input, DateTime.Now);

            Assert.NotNull(result);
            Assert.AreEqual(18.1f, result.Temperature);
            Assert.AreEqual(0.5f, result.Humidity);
        }

        [Test]
        public void Parse_OnlyTempSensor_InvalidSize_ShouldRaiseException()
        {
            var parser = new TemperatureSensorDataParser();
            var input = "18.1;";          


            Assert.Catch<FormatException>(() =>
            {
                parser.Parse(input, DateTime.Now);
            });            
        }

        [Test]
        public void Parse_OnlyTempSensor_InvalidNumbers_ShouldRaiseException()
        {
            var parser = new TemperatureSensorDataParser();
            var input = "18.1;0.dsaa";


            Assert.Catch<ArgumentException>(() =>
            {
                parser.Parse(input, DateTime.Now);
            });
        }
    }
}
