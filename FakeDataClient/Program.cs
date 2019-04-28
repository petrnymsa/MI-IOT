using Microsoft.AspNetCore.SignalR.Client;
using System;
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
    class Program
    {
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
                Console.WriteLine($"Recieved: {msg}");
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
            Console.WriteLine("Press enter to generate random data");
            Console.ReadKey();
            while (true)
            {
                var hum = (float)rnd.NextDouble();
                var temp = (float)(rnd.Next(21, 23) + rnd.NextDouble());

                var snapshot = new TemperatureSensorSnapshot()
                {
                    Temperature = temp,
                    Humidity = hum,
                    Date = DateTime.Now
                };
                Console.WriteLine($"Sending {snapshot}");
                await connection.InvokeAsync<TemperatureSensorSnapshot>("update",snapshot);

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            

            //myHub.Invoke<string>("Send", "HELLO World ").ContinueWith(task => {
            //    if (task.IsFaulted)
            //    {
            //        Console.WriteLine("There was an error calling send: {0}",
            //                          task.Exception.GetBaseException());
            //    }
            //    else
            //    {
            //        Console.WriteLine(task.Result);
            //    }
            //});

            //myHub.On<string>("addMessage", param => {
            //    Console.WriteLine(param);
            //});

            //myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();


            //Console.Read();
        }    
    }
}
