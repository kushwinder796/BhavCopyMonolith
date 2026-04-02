using Bhav.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.Services
{
    public interface IBhavCopyParserService
    {
        Task<List<BhavCopyRaw>> ParseBhavCopyAsync(Stream csvStream, CancellationToken cancellationToken = default);
    }
}
