using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Common
{
    public static class DateTimeExtension
    {
        public static DateTime TruncateMilliseconds(this DateTime dt)
        {
            return dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerSecond);
        }

        public static DateTime? TruncateMilliseconds(this DateTime? dt)
        {
            if (!dt.HasValue)
                return dt;

            return dt.Value.AddTicks(-dt.Value.Ticks % TimeSpan.TicksPerSecond);
        }
    }
}
