
using Bhav.Application.DTOs;
using Bhav.Application.IRepositories;
using Bhav.Application.Services;
using Bhav.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Bhav.Infrastructure.Services
{
    public class BhavUploadService : IBhavUploadService
    {
        private readonly IBhavCopyRepository _repository;
        private readonly ILogger<BhavUploadService> _logger;

        public BhavUploadService(IBhavCopyRepository repository, ILogger<BhavUploadService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UploadResultDto> UploadBhavCopyAsync(List<BhavCopyRaw> parsedData, CancellationToken cancellationToken = default)
        {
            if (parsedData == null || parsedData.Count == 0)
                return new UploadResultDto
                {
                    Success = false,
                    Message = "No data to upload"
                };

            try
            {
                var bhavRecords = new List<BhavCopy>();
                int skippedCount = 0;

                foreach (var raw in parsedData)
                {
                    // Handle nullable TradeDate
                    if (!raw.TradeDate.HasValue)
                    {
                        skippedCount++;
                        _logger.LogWarning($"Record skipped: {raw.SecurityCode} - no trade date");
                        continue;
                    }

                    var tradeDate = raw.TradeDate.Value;

                    // Check if record already exists (using DateTime comparison)
                    var exists = await _repository.ExistsAsync(raw.SecurityCode,tradeDate, cancellationToken);
                    if (exists)
                    {
                        skippedCount++;
                        _logger.LogWarning($"Record already exists: {raw.SecurityCode} on {tradeDate:yyyy-MM-dd}");
                        continue;
                    }

                    bhavRecords.Add(new BhavCopy
                    {
                        
                        Symbol = raw.SecurityCode,
                        SecurityName = raw.SecurityName,
                        OpenPrice = raw.OpenPrice,
                        HighPrice = raw.HighPrice,
                        LowPrice = raw.LowPrice,
                        ClosePrice = raw.ClosePrice,
                        Volume = raw.Volume,
                        TradedValue = raw.TradedValue,
                        TradeDate = tradeDate,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                if (bhavRecords.Count == 0)
                {
                    return new UploadResultDto
                    {
                        Success = false,
                        Message = $"All {skippedCount} records skipped (duplicates or invalid)"
                    };
                }

                // Add to repository
                await _repository.AddRangeAsync(bhavRecords, cancellationToken);

                // Save changes
                var recordsAffected = await _repository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Successfully uploaded {recordsAffected} BHAV records (Skipped: {skippedCount})");

                return new UploadResultDto
                {
                    Success = true,
                    Message = $"Uploaded {recordsAffected} records successfully (Skipped {skippedCount} duplicates)",
                    RecordsInserted = recordsAffected
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"BHAV upload failed: {ex.Message}");
                return new UploadResultDto
                {
                    Success = false,
                    Message = $"Upload failed: {ex.Message}"
                };
            }
        }
    }
}