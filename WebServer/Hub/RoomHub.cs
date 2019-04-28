using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Hub
{
    public class RoomHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public const string UpdateMessage = "roomUpdate";

        [Microsoft.AspNetCore.SignalR.HubMethodName("update")]
        public void SendUpdate(TemperatureSensorSnapshot snapshot)
        {
            Clients.All.SendCoreAsync(UpdateMessage, new[] { snapshot });
        }
    }
}
