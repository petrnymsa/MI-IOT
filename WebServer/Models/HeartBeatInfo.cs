using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class HeartBeatInfo
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int FailsInRow { get; set; }

        public DateTime LastTimeAlive { get; set; }

        public DateTime LastEntry { get; set; }
    }
}
