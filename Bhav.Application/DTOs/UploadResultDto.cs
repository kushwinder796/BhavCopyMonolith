using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.DTOs
{
    public class UploadResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RecordsInserted { get; set; }
    }
}
