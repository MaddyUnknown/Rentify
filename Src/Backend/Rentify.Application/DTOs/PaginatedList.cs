using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs
{
    public class PaginatedList<T>
    {
        public int CurrentPage { get; internal set; }
        public int TotalItems { get; internal set; }
        public IEnumerable<T> Items { get; internal set; }

        public PaginatedList(int currentPage, int totalItems, IEnumerable<T> items)
        {
            CurrentPage = currentPage;
            TotalItems = totalItems;
            Items = items;
        }
    }
}
