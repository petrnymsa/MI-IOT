using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Common;
using WebServer.Data;
using WebServer.Hub;

namespace WebServer
{
    public class HeartBeatHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger<HeartBeatHostedService> _logger;
        private readonly IServiceProvider services;

        const int BeatThresholdSeconds = 60;
        const int CheckIntervalSeconds = 60;

        public HeartBeatHostedService(ILogger<HeartBeatHostedService> logger, IServiceProvider services)
        {
            this._logger = logger;
            this.services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Start");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(CheckIntervalSeconds));

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Stop");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
                var first = context.HeartBeatInfo.FirstOrDefault();
                if (first == null)
                    return;

                var now = DateTime.Now.TruncateMilliseconds();
                var lastAlive = first.LastTimeAlive.TruncateMilliseconds();
                var diff = now - lastAlive;

                if (diff.TotalMilliseconds >= TimeSpan.FromSeconds(BeatThresholdSeconds).TotalMilliseconds)
                {
                    first.FailsInRow++;
                    _logger.LogError($"Device not alive: {first.FailsInRow} times");
                }

                first.LastEntry = now;
                await context.SaveChangesAsync();

                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<RoomHub>>();
                _logger.LogInformation("Sending information over hub");
                await hub.Clients.All.SendAsync(RoomHub.StatusUpdate, first.GetStatus().ToString());
            }
        }

        public void Dispose() => _timer?.Dispose();

    }
}
