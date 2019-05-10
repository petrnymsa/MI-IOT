using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ServerDbContext context;

        public StatusController(ServerDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public HeartBeatStatus Get()
        {
            var first = context.HeartBeatInfo.First();
            return first.GetStatus();           
        }

        [HttpPost]
        public void SetStatus()
        {
            var first = context.HeartBeatInfo.First();
            first.FailsInRow = 0;
            first.LastTimeAlive = DateTime.Now;

            context.SaveChanges();

            //todo Hub - inform status channel

        }
    }
}
