using System;
using System.Collections.Generic;

namespace Bhav.Infrastructure.Persistence.Entities;

public partial class BhavCopy
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateOnly TradeDate { get; set; }

    public decimal? OpenPrice { get; set; }

    public decimal? HighPrice { get; set; }

    public decimal? LowPrice { get; set; }

    public decimal? ClosePrice { get; set; }

    public int? Volume { get; set; }

    public decimal? OpenInterest { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BhavCopyAudit> BhavCopyAudits { get; set; } = new List<BhavCopyAudit>();
}
