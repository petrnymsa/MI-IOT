using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Hub
{
    public class RoomHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public const string RoomUpdate = "roomUpdate";
        public const string FlowerUpdate = "flowerUpdate";
        public const string StatusUpdate = "statusUpdate";

        [Microsoft.AspNetCore.SignalR.HubMethodName(RoomUpdate)]
        public void SendRoomUpdateMessage(TemperatureSensorSnapshot snapshot)
        {
            Clients.All.SendCoreAsync(RoomUpdate, new[] { snapshot });
        }

        [Microsoft.AspNetCore.SignalR.HubMethodName(FlowerUpdate)]
        public void SendFlowerUpdateMessage(HumiditySensorSnapshot snapshot)
        {
            Clients.All.SendCoreAsync(FlowerUpdate, new[] { snapshot });
        }

        [Microsoft.AspNetCore.SignalR.HubMethodName(StatusUpdate)]
        public void SendStatusUpdateMessage(HeartBeatStatus status)
        {
            Clients.All.SendCoreAsync(StatusUpdate,new[] { status.ToString() });
        }
    }
}
