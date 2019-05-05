using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Data
{
    public class PagedHumiditySensorSnapshot : HumiditySensorSnapshot
    {
        public PagingInfo Paging { get; set; }

        public PagedHumiditySensorSnapshot(PagingInfo paging)
        {
            this.Paging = paging;
        }
    }
}
