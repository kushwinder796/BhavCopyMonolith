using Bhav.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.Services
{
    public interface IBhavUploadService
    {
        Task<UploadResultDto> UploadBhavCopyAsync(List<BhavCopyRaw> parsedData, CancellationToken cancellationToken = default);
    }
}
