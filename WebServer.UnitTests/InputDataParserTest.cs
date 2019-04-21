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
            var parser = new InputDataParser();
            var input = "18.1;0.5";
            var result = parser.Parse(input, DateTime.Now);

            Assert.NotNull(result.TemperatureSensor);
            Assert.IsNull(result.HumiditySensor);
            Assert.AreEqual(18.1f, result.TemperatureSensor.Temperature);
            Assert.AreEqual(0.5f, result.TemperatureSensor.Humidity);
        }

        [Test]
        public void Parse_BothSensors_ShouldReturnCorrectly()
        {
            var parser = new InputDataParser();
            var input = "18.1;0.5;0.8";
            var result = parser.Parse(input, DateTime.Now);

            Assert.NotNull(result.TemperatureSensor);
            Assert.NotNull(result.HumiditySensor);
            Assert.AreEqual(18.1f, result.TemperatureSensor.Temperature);
            Assert.AreEqual(0.5f, result.TemperatureSensor.Humidity);
            Assert.AreEqual(0.8f, result.HumiditySensor.Humidity);
        }

        [Test]
        public void Parse_OnlyTempSensor_InvalidSize_ShouldRaiseException()
        {
            var parser = new InputDataParser();
            var input = "18.1;";          


            Assert.Catch<FormatException>(() =>
            {
                parser.Parse(input, DateTime.Now);
            });            
        }

        [Test]
        public void Parse_OnlyTempSensor_InvalidNumbers_ShouldRaiseException()
        {
            var parser = new InputDataParser();
            var input = "18.1;0.dsaa";


            Assert.Catch<ArgumentException>(() =>
            {
                parser.Parse(input, DateTime.Now);
            });
        }
    }
}
