using AirportControlTower.API.Application.Commands;
using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static AirportControlTower.API.Application.Requests.ControlTowerRequests;

namespace AirportControlTower.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlTowerController : ControllerBase
    {
        [HttpPost("{call_sign}/intent")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Intent(string call_sign, [FromBody] RequestStateChange request, [FromServices] IMediator mediator)
        {
            await mediator.Send(new RequestStateChangeCommand { State = request.State, CallSign = call_sign });
            return NoContent();
        }

        [HttpGet("all-airlines")]
        public async Task<IActionResult> AllAirlines([FromServices] AppDbContext dbContext, AirlineState state)
        {
            return Ok(await dbContext.Airlines.AsNoTracking().Where(a => a.State == state).ToListAsync());
        }
    }
}
