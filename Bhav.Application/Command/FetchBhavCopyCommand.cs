using Bhav.Infrastructure.Persistence.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.Command
{
    public class FetchBhavCopyCommand : IRequest<List<BhavCopy>>
    {
        public DateOnly Date { get; set; }

        public FetchBhavCopyCommand(DateOnly date)
        {
            Date = date;
        }
    }
}
