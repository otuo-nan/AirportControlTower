using AirportControlTower.API.Models;

#nullable disable
namespace AirportControlTower.API.Application.Dtos
{
    public class DashboardDto
    {
        public WeatherData LastFetchedWeatherData { get; set; }
        public IEnumerable<ParkingLotViewDto> ParkingLotView { get; set; }
        public  IEnumerable<StateChangeHistoryDto> Last10AirlineStateChangeHistory { get; set; }
    }
}
