﻿using System;
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
    public class FlowerController : ControllerBase
    {
        private readonly ServerDbContext _context;
        private readonly IInputDataParser<HumiditySensorSnapshot> parser;
        private readonly IHubContext<RoomHub> roomHub;

        public FlowerController(ServerDbContext context, IInputDataParser<HumiditySensorSnapshot> inputDataParser, IHubContext<RoomHub> roomHub)
        {
            this._context = context;
            this.parser = inputDataParser;
            this.roomHub = roomHub;
        }

        [HttpGet]
        public ActionResult<List<HumiditySensorSnapshot>> GetLast(int count)
        {
            var last = _context.HumiditySensorSnapshots.Skip(_context.HumiditySensorSnapshots.Count() - count).ToList();
            return Ok(last);
        }


        [HttpPost]
        public async Task<ActionResult> Add()
        {
            var raw = await Request.GetRawBodyStringAsync();
            var snapshot = parser.Parse(raw, DateTime.Now);

            await _context.HumiditySensorSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();

            await roomHub.Clients.All.SendAsync(RoomHub.FlowerUpdate, snapshot);

            return Ok();
        }
    }
}

