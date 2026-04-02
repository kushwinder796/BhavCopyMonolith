using Bhav.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.Command
{
    public class UploadBhavCopyCommand : IRequest<UploadResultDto>
    {
        public required IFormFile File { get; set; }
    }
}
