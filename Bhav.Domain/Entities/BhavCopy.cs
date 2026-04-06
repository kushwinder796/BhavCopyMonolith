using System;
using System.Collections.Generic;

namespace Bhav.Infrastructure.Persistence.Entities;

public partial class BhavCopy
{
    public int Id { get; set; }

    public DateOnly TradeDate { get; set; }

    public string? Exchange { get; set; }

    public string? Segment { get; set; }

    public string Data { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? Symbol { get; set; }
}
