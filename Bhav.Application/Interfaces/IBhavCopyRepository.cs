

using Bhav.Infrastructure.Persistence.Entities;

namespace Bhav.Application.IRepositories
{
    public interface IBhavCopyRepository
    {
        Task<List<string>> GetSymbolsByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

        Task AddRangeAsync(List<BhavCopy> Data);

        Task<List<BhavCopy>> GetByDateAsync(DateOnly date);

        Task<byte[]?> DownloadAsync(string url);
      

    }
}