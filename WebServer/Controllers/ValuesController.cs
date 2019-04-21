using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebServer.Common;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ServerDbContext _context;
        private readonly IInputDataParser inputDataParser;

        public ValuesController(ServerDbContext context, IInputDataParser inputDataParser)
        {
            this._context = context;
            this.inputDataParser = inputDataParser;
        }

        [HttpGet]
        public List<TemperatureSensorSnapshot> Get()
        {
            var result = inputDataParser.Parse("24.5;0.8;0.1f", DateTime.Now);
            var snapshots = _context.TemperatureSensorSnapshots.ToList();
            return snapshots;
        }

        [HttpPost]
        public async Task<ActionResult> Add()
        {
            var raw = await Request.GetRawBodyStringAsync();


            return Ok();
        }
    }
}