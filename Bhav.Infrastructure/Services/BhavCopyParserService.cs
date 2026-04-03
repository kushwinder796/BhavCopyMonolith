using Bhav.Application.DTOs;
using Bhav.Application.Services;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Bhav.Infrastructure.Services
{
    public class BhavCopyParserService : IBhavCopyParserService
    {
        public async Task<List<BhavCopyRaw>> ParseBhavCopyAsync(Stream csvStream, DateTime tradeDate, CancellationToken 
            cancellationToken = default)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim,
            };

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<BhavCopyMap>();

            var records = await Task.FromResult(csv.GetRecords<BhavCopyRaw>().ToList());

            // Set TradeDate on every record from the filename date
            foreach (var record in records)
            {
                record.TradeDate = tradeDate;
            }

            return records;
        }
    }

    public sealed class BhavCopyMap : ClassMap<BhavCopyRaw>
    {
        public BhavCopyMap()
        {
            Map(m => m.SecurityCode).Name("SYMBOL");
            Map(m => m.SecurityName).Name("SECURITY");
            Map(m => m.OpenPrice).Name("OPEN_PRICE");
            Map(m => m.HighPrice).Name("HIGH_PRICE");
            Map(m => m.LowPrice).Name("LOW_PRICE");
            Map(m => m.ClosePrice).Name("CLOSE_PRICE");
            Map(m => m.Volume).Name("NET_TRDQTY");
            Map(m => m.TradedValue).Name("NET_TRDVAL");
            Map(m => m.TradeDate).Ignore();
        }
    }
}