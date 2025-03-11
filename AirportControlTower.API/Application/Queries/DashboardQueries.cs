using AirportControlTower.API.Application.Dtos;

namespace AirportControlTower.API.Application.Queries
{
    public class DashboardQueries(StateChangeHistoryQueries historyQueries,
        WeatherQueries weatherQueries,
        AirlineQueries airlineQueries)
    {
        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            return new DashboardDto
            {
                LastFetchedWeatherData = await weatherQueries.GetLastFetchedWeatherAsync(),
                ParkingLotView = await airlineQueries.GetParkingLotViewAsync(),
                Last10AirlineStateChangeHistory = await historyQueries.Last_N_AirlineStateChangeHistoryAsync(10)
            };

        }
    }
}
