
using Bhav.Application.Command;
using Bhav.Application.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BhavCopyProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BhavController : ControllerBase
    {
        private readonly IMediator _mediator;
        

        public BhavController( IMediator mediator)
        {
            _mediator = mediator;
           
           
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime date)
        {
            var dateOnly = DateOnly.FromDateTime(date);

            var result = await _mediator.Send(new FetchBhavCopyCommand(dateOnly));

            return Ok(result);
        }
    }
}