// Path: Modules/Bhav/Bhav.Infrastructure/Repositories/BhavCopyRepository.cs

using Bhav.Application.IRepositories;
using Bhav.Domain.Entities;
using Bhav.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bhav.Infrastructure.Repositories
{
    public class BhavCopyRepository : IBhavCopyRepository
    {
        private readonly BhavDbContext _context;

        public BhavCopyRepository(BhavDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<string>> GetSymbolsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.BhavCopies
                .Where(x => x.TradeDate.HasValue && x.TradeDate.Value.Date == date.Date && x.IsActive == true)
                .Select(x => x.Symbol)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<BhavCopy> records, CancellationToken cancellationToken = default)
        {
            if (records == null || !records.Any())
                throw new ArgumentException("Records cannot be null or empty", nameof(records));

            await _context.BhavCopies.AddRangeAsync(records, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string symbol, DateTime tradeDate, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

            return await _context.BhavCopies
                .AnyAsync(x => x.Symbol == symbol
                    && x.TradeDate.HasValue
                    && x.TradeDate.Value.Date == tradeDate.Date
                    && x.IsActive == true,
                    cancellationToken);
        }

        public async Task<List<BhavCopy>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            if (fromDate > toDate)
                throw new ArgumentException("FromDate cannot be greater than ToDate");

            return await _context.BhavCopies
                .Where(x => x.TradeDate.HasValue
                    && x.TradeDate.Value.Date >= fromDate.Date
                    && x.TradeDate.Value.Date <= toDate.Date
                    && x.IsActive == true)
                .OrderByDescending(x => x.TradeDate)
                .ThenBy(x => x.Symbol)
                .ToListAsync(cancellationToken);
        }

        public async Task<BhavCopy> GetBySymbolAndDateAsync(string symbol, DateTime tradeDate, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

            return await _context.BhavCopies
                .FirstOrDefaultAsync(x => x.Symbol == symbol
                    && x.TradeDate.HasValue
                    && x.TradeDate.Value.Date == tradeDate.Date
                    && x.IsActive == true,
                    cancellationToken);
        }

        public async Task<(List<BhavCopy>, int)> GetPaginatedAsync(  DateTime? fromDate, DateTime? toDate,string symbol,int pageNumber,int pageSize,CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1)
                throw new ArgumentException("PageNumber must be >= 1", nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentException("PageSize must be >= 1", nameof(pageSize));

            var query = _context.BhavCopies.Where(x => x.IsActive == true);

            if (fromDate.HasValue)
                query = query.Where(x => x.TradeDate.HasValue && x.TradeDate.Value.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.TradeDate.HasValue && x.TradeDate.Value.Date <= toDate.Value.Date);

            if (!string.IsNullOrEmpty(symbol))
                query = query.Where(x => x.Symbol.Contains(symbol));

            var totalCount = await query.CountAsync(cancellationToken);

            var records = await query
                .OrderByDescending(x => x.TradeDate)
                .ThenBy(x => x.Symbol)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (records, totalCount); 
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}