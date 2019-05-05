using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Data
{
    public class PagingInfo
    {
        public int CurrentPage { get; }  
        public int CurrentItemsCount { get; }

        public int TotalItems { get; }
        public int MaxPages { get; }

        public PagingInfo(int currentPage, int currentItemsCount, int maxPages, int totalItems)
        {
            CurrentPage = currentPage;
            CurrentItemsCount = currentItemsCount;
            TotalItems = totalItems;
            MaxPages = maxPages;
        }
    }
}
