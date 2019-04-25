using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebServer.Data;

namespace WebServer
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    CreateWebHostBuilder(args).Build().Run();
        //}

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();


        public static void Main(string[] args)
        {
            var ip = GetLocalIPAddress();
            var host  = CreateWebHostBuilder(args, ip).Build();


            // use this to allow command line parameters in the config
            //var configuration = new ConfigurationBuilder()
            //    .AddCommandLine(args)
            //    .Build();


            //   var hostUrl = configuration["hosturl"];
            //   if (string.IsNullOrEmpty(hostUrl))
            //     hostUrl = "http://0.0.0.0:6000";


            //var host = new WebHostBuilder()
            //    .UseKestrel()
            //    .UseUrls("http://localhost:5000", $"http://{ip}:5000")   // <!-- this 
            //    .UseContentRoot(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
            //    .UseIISIntegration()
            //    .UseStartup<Startup>()
            //    .ConfigureLogging((hostingContext, logging) =>
            //    {
            //        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            //        logging.AddConsole();
            //        logging.AddDebug();
            //        logging.AddEventSourceLogger();
            //    })
            //    .UseConfiguration(configuration)
            //    .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ServerDbContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string ipAdress = "") =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5000", $"http://{ipAdress}:5000")
                .UseStartup<Startup>();

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }


}
