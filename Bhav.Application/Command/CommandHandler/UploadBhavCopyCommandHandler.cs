using Bhav.Application.DTOs;
using Bhav.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bhav.Application.Command
{
    public class UploadBhavCopyCommandHandler : IRequestHandler<UploadBhavCopyCommand, UploadResultDto>
    {
        private readonly IBhavCopyParserService _parserService;
        private readonly IBhavUploadService _uploadService;
        private readonly ILogger<UploadBhavCopyCommandHandler> _logger;

        public UploadBhavCopyCommandHandler(
            IBhavCopyParserService parserService,
            IBhavUploadService uploadService,
            ILogger<UploadBhavCopyCommandHandler> logger)
        {
            _parserService = parserService ?? throw new ArgumentNullException(nameof(parserService));
            _uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UploadResultDto> Handle(UploadBhavCopyCommand command, CancellationToken cancellationToken)
        {
            if (command.File == null || command.File.Length == 0)
                return new UploadResultDto { Success = false, Message = "File is required" };

            if (!command.File.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return new UploadResultDto { Success = false, Message = "Only CSV files are allowed" };

          
            var fileName = Path.GetFileNameWithoutExtension(command.File.FileName);
            var dateMatch = Regex.Match(fileName, @"\d{8}");

            if (!dateMatch.Success ||
                !DateTime.TryParseExact(dateMatch.Value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var tradeDate))
            {
                return new UploadResultDto
                {
                    Success = false,
                    Message = "Could not extract trade date from filename. Expected format: BhavCopy_DDMMYYYY.csv"
                };
            }

            _logger.LogInformation("Uploading Bhav Copy for date: {Date}", tradeDate.ToString("yyyy-MM-dd"));

            try
            {
                using var stream = command.File.OpenReadStream();

                var parsedData = await _parserService.ParseBhavCopyAsync(stream, tradeDate, cancellationToken);

                if (parsedData == null || parsedData.Count == 0)
                    return new UploadResultDto { Success = false, Message = "CSV file is empty" };

                return await _uploadService.UploadBhavCopyAsync(parsedData, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload error for file: {FileName}", command.File.FileName);
                return new UploadResultDto
                {
                    Success = false,
                    Message = $"Upload failed: {ex.Message}"
                };
            }
        }
    }
}