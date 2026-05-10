using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.Backend.Helpers
{
    public class QueryObject
    {
        public string? SearchTerm { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public bool IsSortDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? UserId { get; set; } = null;
    }
}