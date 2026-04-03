using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.DTOs
{
    public class BhavCopyRaw
    {
        public string SecurityCode { get; set; } = string.Empty;  
        public string SecurityName { get; set; } = string.Empty; 
        public decimal? OpenPrice { get; set; }
        public decimal? HighPrice { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public long? Volume { get; set; }       
        public decimal? TradedValue { get; set; }
        public DateTime? TradeDate { get; set; }
    }
    
}
