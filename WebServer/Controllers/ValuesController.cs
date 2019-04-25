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
      //  private readonly IInputDataParser inputDataParser;

        static List<string> raws = new List<string>();

        public ValuesController(ServerDbContext context)
        {
            this._context = context;
         //   this.inputDataParser = inputDataParser;
        }

        [HttpGet]
        public List<string> Get()
        {

            //  var snapshots = _context.TemperatureSensorSnapshots.ToList();
            // return snapshots;
            return raws;
        }

        [HttpPost]
        public async Task<ActionResult> Add()
        {
            var raw = await Request.GetRawBodyStringAsync();
            raws.Add(raw);

            return Ok();
        }
    }
}