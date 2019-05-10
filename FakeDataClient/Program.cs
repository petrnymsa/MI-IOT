using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FakeDataClient
{

    public class TemperatureSensorSnapshot
    {
        public int Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }

        public DateTime Date { get; set; }

        public override string ToString() => $"{Id};{Temperature};{Humidity};{Date}";
    }

    public class HumiditySensorSnapshot
    {
        public int Id { get; set; }
        public float Humidity { get; set; }

        public DateTime Date { get; set; }

        public override string ToString() => $"{Id};{Humidity};{Date}";

    }
    class Program
    {

        static void Print(string msg, ConsoleColor color)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = c;
        }
        static async Task Main(string[] args)
        {
            //Set connection         
            var rnd = new Random();
            var connection = new HubConnectionBuilder()
                  .WithUrl("http://localhost:5000/hub/room")
                  .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };            

            connection.On<TemperatureSensorSnapshot>("roomUpdate", (msg) =>
            {            
                Print($"Recieved: {msg}", ConsoleColor.Green);             
            });

            connection.On<TemperatureSensorSnapshot>("flowerUpdate", (msg) =>
            {
                Print($"Recieved: {msg}", ConsoleColor.Cyan);
            });

            connection.On<string>("statusUpdate", (msg) =>
            {
                Print($"Recieved status update: {msg}", ConsoleColor.Magenta);
                
            });

            try
            {
                await connection.StartAsync();               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Connected");

            //var roomTimer = new Timer((t) =>
            //{
            //    SendRoom(rnd, connection);
            //},null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));

            //var flowerTimer = new Timer((t) =>
            //{
            //    SendFlower(rnd, connection);
            //}, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(2000));

            Console.WriteLine("Press key to send update");
            using (var client = new HttpClient())
            {
                while (true)
                {
                    Console.ReadKey();

                    await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/status"));

                }
            }

            Console.WriteLine("--- PRESS KEY TO EXIT ---");
            Console.ReadKey();
        }

        private static async Task SendRoom(Random rnd, HubConnection connection)
        {
            var hum = (float)rnd.NextDouble();
            var temp = (float)(rnd.Next(21, 23) + rnd.NextDouble());

            var snapshot = new TemperatureSensorSnapshot()
            {
                Temperature = temp,
                Humidity = hum,
                Date = DateTime.Now
            };
           Print($"Sending {snapshot}", ConsoleColor.Blue);
            await connection.InvokeAsync<TemperatureSensorSnapshot>("roomUpdate", snapshot);
        }

        private static async Task SendFlower(Random rnd, HubConnection connection)
        {
            var hum = (float)rnd.NextDouble();

            var snapshot = new HumiditySensorSnapshot()
            {
                Humidity = hum,
                Date = DateTime.Now
            };
            Print($"Sending {snapshot}", ConsoleColor.Magenta);
            await connection.InvokeAsync<TemperatureSensorSnapshot>("flowerUpdate", snapshot);
        }
    }
}
