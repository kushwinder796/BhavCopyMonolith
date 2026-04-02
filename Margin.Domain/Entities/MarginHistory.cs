using System;
using System.Collections.Generic;

namespace Margin.Infrastructure.Persistence.Entities;

public partial class MarginHistory
{
    public int Id { get; set; }

    public int MarginId { get; set; }

    public string Symbol { get; set; }

    public DateOnly? TradeDate { get; set; }

    public decimal? TotalMargin { get; set; }

    public DateTime? RecordedAt { get; set; }

    public virtual MarginDetail Margin { get; set; }
}
