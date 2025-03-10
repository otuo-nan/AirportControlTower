using AirportControlTower.API.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AirportControlTower.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        [HttpGet("{call_sign}/weather")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Intent(string call_sign, [FromServices] WeatherService service)
        {
            return Ok(await service.GetWeatherInformationAsync(call_sign));
        }
    }
}
