using Bhav.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.Queries
{
    public class GetBhavByDateQuery : IRequest<GetBhavByDateQueryResponse>
    {
        public DateTime Date { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetBhavByDateQueryResponse
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<BhavResponseDto> Data { get; set; } = new();
    }
}
