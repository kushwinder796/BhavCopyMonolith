using System;
using System.Collections.Generic;

namespace Bhav.Infrastructure.Persistence.Entities;

public partial class BhavCopyAudit
{
    public int Id { get; set; }

    public int BhavCopyId { get; set; }

    public string Operation { get; set; }

    public string OldValues { get; set; }

    public string NewValues { get; set; }

    public string ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual BhavCopy BhavCopy { get; set; }
}
