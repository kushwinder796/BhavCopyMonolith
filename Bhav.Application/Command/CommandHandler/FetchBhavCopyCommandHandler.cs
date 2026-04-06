
using Bhav.Application.IRepositories;
using Bhav.Application.Services;
using Bhav.Infrastructure.Persistence.Entities;
using MediatR;
using System.Globalization;

namespace Bhav.Application.Command.CommandHandler;

public class FetchBhavCopyCommandHandler : IRequestHandler<FetchBhavCopyCommand, List<BhavCopy>>
{
    private readonly IBhavCopyRepository _repo;
   
    private readonly BhavCopyParserService _parser;

    public FetchBhavCopyCommandHandler( IBhavCopyRepository repo,BhavCopyParserService parser)
    {
        _repo = repo;
        
        _parser = parser;
    }

    public async Task<List<BhavCopy>> Handle(FetchBhavCopyCommand request, CancellationToken cancellationToken)
    {
   
        if (request.Date.DayOfWeek == DayOfWeek.Saturday ||
            request.Date.DayOfWeek == DayOfWeek.Sunday)
        {
            throw new Exception($"Market closed on {request.Date}");
        }

  
        if (request.Date == DateOnly.FromDateTime(DateTime.Today))
        {
            request.Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); 
        }


        var existing = await _repo.GetByDateAsync(request.Date);
        if (existing.Any()) return existing;

        var allData = new List<BhavCopy>();
        var urls = GenerateDailyUrls(request.Date);

        foreach (var u in urls)
        {
            try
            {
                var bytes = await _repo.DownloadAsync(u.Url);

                if (bytes == null) continue;

                List<Dictionary<string, object>> parsed;

                if (u.Url.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    parsed = await _parser.ParseZipAsync(bytes);
                }
                else if (u.Url.EndsWith(".DAT", StringComparison.OrdinalIgnoreCase))
                {
                    parsed = await _parser.ParseDelimitedAsync(bytes, '|');
                }
                else
                {
                    parsed = await _parser.ParseDelimitedAsync(bytes, ',');
                }

               
                if (parsed.Any())
                {
                    Console.WriteLine($"[{u.Exchange}-{u.Segment}] Columns: {string.Join(", ", parsed[0].Keys)}");
                }

                int added = 0;
                foreach (var record in parsed)
                {
                    string symbol = ExtractSymbol(record, u.Exchange, u.Segment);

                    if (string.IsNullOrEmpty(symbol))
                    {
                        if (added == 0)
                            Console.WriteLine($"[{u.Exchange}-{u.Segment}]  Symbol null. Keys: {string.Join(", ", record.Keys)}");
                        continue;
                    }

                    allData.Add(new BhavCopy
                    {
                        TradeDate = request.Date,
                        Exchange = u.Exchange,
                        Segment = u.Segment,
                        Symbol = symbol,
                        Data = System.Text.Json.JsonSerializer.Serialize(record)
                    });
                    added++;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {u.Exchange}-{u.Segment} → {ex.Message}");
            }
        }

        if (allData.Any())
        {
            await _repo.AddRangeAsync(allData); 
        }

        return allData;
    }

  
    private static string? ExtractSymbol(Dictionary<string, object> record, string exchange, string segment)
    {
       
        var candidates = new List<string>();

        if (exchange == "NSE")
        {
            candidates = new List<string> { "SYMBOL", "SECURITY", "Symbol", "STOCK_SYMBOL", "SCRIP_CD" };
        }
        else if (exchange == "BSE")
        {
            candidates = new List<string> { "SC_CODE", "TckrSymb", "SCRIP_CD", "SECURITY_CODE", "Scrip Code", "SC_NAME" };
        }

        // Try exact match first
        foreach (var col in candidates)
        {
            if (record.TryGetValue(col, out var val))
            {
                var s = val?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(s)) return s;
            }
        }

        
        foreach (var col in candidates)
        {
            var key = record.Keys.FirstOrDefault(k =>
                k.Equals(col, StringComparison.OrdinalIgnoreCase));
            if (key != null)
            {
                var s = record[key]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(s)) return s;
            }
        }

        return null;
    }

    private List<(string Url, string Exchange, string Segment)> GenerateDailyUrls(DateOnly date)
    {
        var ddMMyy = date.ToString("ddMMyy");       
        var ddMMyyyy = date.ToString("ddMMyyyy");   
        var yyyyMMdd = date.ToString("yyyyMMdd");  

        return new List<(string, string, string)>
    {
        
        ($"https://nsearchives.nseindia.com/content/trdops/FNO_BC{ddMMyyyy}.DAT",
         "NSE", "FO"),

        //  NSE CASH (PR ZIP)
        ($"https://nsearchives.nseindia.com/archives/equities/bhavcopy/pr/PR{ddMMyy}.zip",
         "NSE", "CASH"),

        //  BSE FO
        ($"https://bseindia.com/download/Bhavcopy/Derivative/BhavCopy_BSE_FO_0_0_0_{yyyyMMdd}_F_0000.CSV",
         "BSE", "FO"),

        //  BSE CASH
        ($"https://bseindia.com/download/BhavCopy/Equity/BhavCopy_BSE_CM_0_0_0_{yyyyMMdd}_F_0000.CSV",
         "BSE", "CASH")
    };  
}
}