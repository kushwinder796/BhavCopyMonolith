using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.DTOs
{
    public class PaginatedResultDto<T>
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new();
    }
}
