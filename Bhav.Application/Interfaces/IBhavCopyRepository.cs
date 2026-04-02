

using Bhav.Domain.Entities;

namespace Bhav.Application.IRepositories
{
    public interface IBhavCopyRepository
    {
        Task<List<string>> GetSymbolsByDateAsync(DateTime date, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<BhavCopy> records, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string symbol, DateTime tradeDate, CancellationToken cancellationToken = default);

        Task<List<BhavCopy>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

        Task<BhavCopy?> GetBySymbolAndDateAsync(string symbol, DateTime tradeDate, CancellationToken cancellationToken = default);

        Task<(List<BhavCopy> records, int totalCount)> GetPaginatedAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string symbol,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}