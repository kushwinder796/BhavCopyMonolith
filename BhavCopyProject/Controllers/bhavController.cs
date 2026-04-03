

using Bhav.Application.Command;
using Bhav.Application.DTOs;
using Bhav.Application.IRepositories;
using Bhav.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BhavCopyProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BhavController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBhavCopyRepository _repository;
        private readonly ILogger<BhavController> _logger;

        public BhavController( IMediator mediator,IBhavCopyRepository repository,ILogger<BhavController> logger)
        {
            _mediator = mediator;
            _repository = repository;
            _logger = logger;
        }

  

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto request)
        {
            var result = await _mediator.Send(
                new UploadBhavCopyCommand { File = request.File });

            return Ok(result);
        }

        [HttpGet("by-date/{date}")]
        public async Task<IActionResult> GetBhavByDate(string date,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!DateTime.TryParse(date, out var parsedDate))
                    return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd" });

                var query = new GetBhavByDateQuery
                {
                    Date = parsedDate,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Query error: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching data", error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBhav(
            [FromQuery] string fromDate,
            [FromQuery] string toDate,
            [FromQuery] string symbol = "",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            try
            {
                DateTime? parsedFromDate = null;
                DateTime? parsedToDate = null;

                if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fDate))
                    parsedFromDate = fDate;

                if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var tDate))
                    parsedToDate = tDate;

                var (records, totalCount) = await _repository.GetPaginatedAsync(
                    fromDate: parsedFromDate,
                    toDate: parsedToDate,
                    symbol: symbol,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    cancellationToken: ct);

                return Ok(new
                {
                    totalRecords = totalCount,
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    data = records.Select(r => new
                    {
                        r.Id,
                        symbol = r.Symbol,
                        r.SecurityName,
                        r.OpenPrice,
                        r.HighPrice,
                        r.LowPrice,
                        r.ClosePrice,
                        r.Volume,
                        r.TradedValue,
                        r.TradeDate
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Query error: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching data", error = ex.Message });
            }
        }


        [HttpGet("symbols/{date}")]
        public async Task<IActionResult> GetSymbolsByDate( string date,CancellationToken ct = default)
        {
            try
            {
                if (!DateTime.TryParse(date, out var parsedDate))
                    return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd" });

                var symbols = await _repository.GetSymbolsByDateAsync(parsedDate, ct);
                return Ok(new { totalSymbols = symbols.Count, symbols });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Query error: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching symbols", error = ex.Message });
            }
        }
    }
}