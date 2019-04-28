using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebServer.Common;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowerController : ControllerBase
    {
        private readonly ServerDbContext _context;
        private readonly IInputDataParser<HumiditySensorSnapshot> parser;

        public FlowerController(ServerDbContext context, IInputDataParser<HumiditySensorSnapshot> inputDataParser)
        {
            this._context = context;
            this.parser = inputDataParser;
        }

        [HttpGet]
        public List<HumiditySensorSnapshot> Get()
        {
            return _context.HumiditySensorSnapshots.ToList();
        }     

        [HttpPost]
        public async Task<ActionResult> Add()
        {
            var raw = await Request.GetRawBodyStringAsync();
            var snapshot = parser.Parse(raw, DateTime.Now);

            await _context.HumiditySensorSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

