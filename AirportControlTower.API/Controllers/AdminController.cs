using AirportControlTower.API.Application.Dtos;
using AirportControlTower.API.Application.Queries;
using AirportControlTower.API.Application.Requests;
using AirportControlTower.API.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AirportControlTower.API.Controllers
{
    [ApiKeyAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("airline-contacts")]
        public async Task<IActionResult> AirlinesThatHaveMadeContactToControlTower(
            [FromQuery] PageRequest request, [FromServices] AirlineQueries queries)
        {
            var options = new PagingOptions(request);

            var (data, count) = await queries.GetAirlinesAsync(request.Page, request.PageSize,
                request.SearchQuery, request.SortField, request.SortDirection);

            options.SetUpRestOfDto(count);

            return Ok(new PaginatedEntities<PagingOptions, ListAirlineDto>
            {
                Data = data,
                PagingOptions = options,
            });
        }

        [HttpGet("airline-state-change-reqest-history")]
        public async Task<IActionResult> AirlineStateChangeHistory(
            [FromQuery] PageRequest request, [FromServices] StateChangeHistoryQueries queries)
        {
            var options = new PagingOptions(request);

            var (data, count) = await queries.GetHistoryAsync(request.Page, request.PageSize,
                request.SearchQuery, request.SortField, request.SortDirection);

            options.SetUpRestOfDto(count);

            return Ok(new PaginatedEntities<PagingOptions, StateChangeHistoryDto>
            {
                Data = data,
                PagingOptions = options,
            });
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData([FromServices] DashboardQueries queries)
        {
            return Ok(await queries.GetDashboardDataAsync());
        }


        [HttpGet("last-n-airline-state-change-reqest-history")]
        public async Task<IActionResult> Last_N_AirlineStateChangeHistory([FromServices] StateChangeHistoryQueries queries, int n = 10)
        {
            return Ok(await queries.Last_N_AirlineStateChangeHistoryAsync(n));
        }
        
        [HttpGet("last-fetched-weather")]
        public async Task<IActionResult> GetLastFetchedWeather([FromServices] WeatherQueries queries)
        {
            return Ok(await queries.GetLastFetchedWeatherAsync());
        }
        
        [HttpGet("parking-lot-view")]
        public async Task<IActionResult> GetParkingLotView([FromServices] AirlineQueries queries)
        {
            return Ok(await queries.GetParkingLotViewAsync());
        }
       
    }
}
