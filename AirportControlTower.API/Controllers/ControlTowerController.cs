using AirportControlTower.API.Application.Commands;
using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirportControlTower.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlTowerController : ControllerBase
    {
        [HttpPost("{call_sign}/intent")]
        public async Task<IActionResult> Intent(string call_sign, [FromBody]RequestChangeCommand command, [FromServices] IMediator mediator)
        {
            command.CallSign = call_sign;
            await mediator.Send(command);
            return NoContent();
        }

        [HttpGet("all airlines")]
        public async Task<IActionResult> AllAirlines([FromServices] AppDbContext dbContext, AirlineState state)
        {
            return Ok(await dbContext.Airlines.AsNoTracking().Where(a => a.State == state).ToListAsync());
        }
    }
}
