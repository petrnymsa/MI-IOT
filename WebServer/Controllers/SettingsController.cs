using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ServerDbContext context;

        public SettingsController(ServerDbContext context)
        {
            this.context = context;
        }

        [HttpGet("/api/[controller]/plain")]
        public ContentResult Get()
        {
            return Content(this.context.Settings.First().ToString());
        }

        [HttpGet]
        public Settings GetJson()
        {
            return this.context.Settings.First();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Settings settings)
        {
            this.context.Settings.Attach(settings).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
