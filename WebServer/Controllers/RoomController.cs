using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebServer.Common;
using WebServer.Data;
using WebServer.Hub;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ServerDbContext _context;
        private readonly IInputDataParser<TemperatureSensorSnapshot> parser;
        private readonly IHubContext<RoomHub> roomHub;

        public RoomController(ServerDbContext context, IInputDataParser<TemperatureSensorSnapshot> inputDataParser, IHubContext<RoomHub> roomHub)
        {
            this._context = context;
            this.parser = inputDataParser;
            this.roomHub = roomHub;
        }

        [HttpGet]
        public ActionResult<List<HumiditySensorSnapshot>> GetWithinInterval(DateTime? from, DateTime? end)
        {
            from = from.TruncateMilliseconds();
            end = end.TruncateMilliseconds();

            if (!from.HasValue && !end.HasValue)
                return Ok(_context.TemperatureSensorSnapshots.ToList());

            if (!end.HasValue)
            {               
                var fromRes = _context.TemperatureSensorSnapshots.Where(x => x.Date.TruncateMilliseconds() >= from.Value).ToList();
                return Ok(fromRes);
            }

            if (!@from.HasValue)
            {
                var endRes = _context.TemperatureSensorSnapshots.Where(x => x.Date.TruncateMilliseconds() <= end.Value).ToList();
                return Ok(endRes);
            }

            var res = _context.TemperatureSensorSnapshots.Where(x => x.Date.TruncateMilliseconds() >= from.Value && x.Date.TruncateMilliseconds() <= end.Value).ToList();
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> Add()
        {
            var raw = await Request.GetRawBodyStringAsync();
            var snapshot = parser.Parse(raw, DateTime.Now);

            await _context.TemperatureSensorSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();

            await roomHub.Clients.All.SendAsync(RoomHub.RoomUpdate, snapshot);

            return Ok();
        }
    }
}