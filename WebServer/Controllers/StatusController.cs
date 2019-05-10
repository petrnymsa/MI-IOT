using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Data;
using WebServer.Hub;
using WebServer.Models;

namespace WebServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ServerDbContext context;
        private readonly IHubContext<RoomHub> hubContext;

        public StatusController(ServerDbContext context, IHubContext<RoomHub> hubContext)
        {
            this.context = context;
            this.hubContext = hubContext;
        }
        [HttpGet]
        public string Get()
        {
            var first = context.HeartBeatInfo.First();
            return first.GetStatus().ToString();           
        }

        [HttpPost]
        public async Task SetStatus()
        {
            var first = context.HeartBeatInfo.First();
            first.FailsInRow = 0;
            first.LastTimeAlive = DateTime.Now;

            await context.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync(RoomHub.StatusUpdate, first.GetStatus().ToString());

        }
    }
}
