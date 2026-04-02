
using Bhav.Application.DTOs;
using Bhav.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bhav.Application.Command
{
    public class UploadBhavCopyCommandHandler : IRequestHandler<UploadBhavCopyCommand, UploadResultDto>
    {
        private readonly IBhavCopyParserService _parserService;
        private readonly IBhavUploadService _uploadService;
        private readonly ILogger<UploadBhavCopyCommandHandler> _logger;

        public UploadBhavCopyCommandHandler(IBhavCopyParserService parserService, IBhavUploadService uploadService,
            ILogger<UploadBhavCopyCommandHandler> logger)
        {
            _parserService = parserService ?? throw new ArgumentNullException(nameof(parserService));
            _uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UploadResultDto> Handle(UploadBhavCopyCommand command, CancellationToken cancellationToken)
        {
            if (command.File == null || command.File.Length == 0)
                return new UploadResultDto
                {
                    Success = false,
                    Message = "File is required"
                };

            if (!command.File.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return new UploadResultDto
                {
                    Success = false,
                    Message = "Only CSV files are allowed"
                };

            try
            {
                using (var stream = command.File.OpenReadStream())
                {
                    // Parse CSV
                    var parsedData = await _parserService.ParseBhavCopyAsync(stream, cancellationToken);

                    if (parsedData == null || parsedData.Count == 0)
                        return new UploadResultDto
                        {
                            Success = false,
                            Message = "CSV file is empty"
                        };

                    // Upload to database
                    var result = await _uploadService.UploadBhavCopyAsync(parsedData, cancellationToken);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Upload error: {ex.Message}");
                return new UploadResultDto
                {
                    Success = false,
                    Message = $"Upload failed: {ex.Message}"
                };
            }
        }
    }
}