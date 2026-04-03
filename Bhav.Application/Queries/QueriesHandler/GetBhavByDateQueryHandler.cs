
using Bhav.Application.DTOs;
using Bhav.Application.IRepositories;
using MediatR;

namespace Bhav.Application.Queries
{
    public class GetBhavByDateQueryHandler : IRequestHandler<GetBhavByDateQuery, GetBhavByDateQueryResponse>
    {
        private readonly IBhavCopyRepository _context;

        public GetBhavByDateQueryHandler(IBhavCopyRepository context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetBhavByDateQueryResponse> Handle(GetBhavByDateQuery query, CancellationToken cancellationToken)
        {
           
            var result = await _context.GetPaginatedAsync(
                fromDate: query.Date.Date,
                toDate: query.Date.Date,
                symbol: string.Empty,
                pageNumber: query.PageNumber,
                pageSize: query.PageSize,
                cancellationToken: cancellationToken);

            var records = result.Item1;  
            var totalCount = result.Item2;

            return new GetBhavByDateQueryResponse
            {
                TotalRecords = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)query.PageSize),
                Data = records.Select(r => new BhavResponseDto
                {
                    Id = r.Id,
                    SecurityCode = r.Symbol,
                    SecurityName = r.SecurityName,
                    OpenPrice = r.OpenPrice,
                    HighPrice = r.HighPrice,
                    LowPrice = r.LowPrice,
                    ClosePrice = r.ClosePrice,
                    Volume = r.Volume,
                    TradedValue = r.TradedValue,
                    TradeDate = r.TradeDate
                }).ToList()
            };
        }
    }
}