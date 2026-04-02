
using Bhav.Application.DTOs;
using Bhav.Application.Services;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Bhav.Infrastructure.Services
{
    public class BhavCopyParserService : IBhavCopyParserService
    {
        public async Task<List<BhavCopyRaw>> ParseBhavCopyAsync(Stream csvStream, CancellationToken cancellationToken = default)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions=TrimOptions.Trim,
            };

            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<BhavCopyMap>();
                var records = await Task.FromResult(csv.GetRecords<BhavCopyRaw>().ToList());
                return records;
            }
        }
    }


    public sealed class BhavCopyMap : ClassMap<BhavCopyRaw>
    {
        public BhavCopyMap()
        {
            // Map CSV column names to DTO properties
            Map(m => m.SecurityCode).Name("ISIN");
            Map(m => m.SecurityName).Name("SecurityName");
            Map(m => m.OpenPrice).Name("Open");
            Map(m => m.HighPrice).Name("High");
            Map(m => m.LowPrice).Name("Low");
            Map(m => m.ClosePrice).Name("Close");
            Map(m => m.Volume).Name("TrdQty");
            Map(m => m.TradedValue).Name("TrdVal");
            Map(m => m.TradeDate).Name("Date");
        }
    }
}