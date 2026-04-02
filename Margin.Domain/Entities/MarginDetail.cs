using System;
using System.Collections.Generic;

namespace Margin.Domain.Entities;

public partial class MarginDetail
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateOnly TradeDate { get; set; }

    public decimal? InitialMargin { get; set; }

    public decimal? ExtremeMargin { get; set; }

    public decimal? VarMargin { get; set; }

    public decimal? TotalMargin { get; set; }

    public decimal? MarginPercentage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<MarginHistory> MarginHistories { get; set; } = new List<MarginHistory>();
}
