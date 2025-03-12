using AirportControlTower.API.Application.Dtos;
using AirportControlTower.API.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace AirportControlTower.API.Application.Queries
{
    public class DashboardQueries(
        IOptions<AirportSpecs> airportOptions,
        StateChangeHistoryQueries historyQueries,
        WeatherQueries weatherQueries,
        AirlineQueries airlineQueries)
    {
        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            return new DashboardDto
            {
                AirportInformation = airportOptions.Value,
                LastFetchedWeatherData = await weatherQueries.GetLastFetchedWeatherAsync(),
                ParkingLotView = await airlineQueries.GetParkingLotViewAsync(),
                Last10AirlineStateChangeHistory = await historyQueries.Last_N_AirlineStateChangeHistoryAsync(10)
            };

        }
    }
}
