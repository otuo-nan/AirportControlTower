using AirportControlTower.API.Infrastructure.Configurations;
using AirportControlTower.API.Models;

#nullable disable
namespace AirportControlTower.API.Application.Dtos
{
    public class DashboardDto
    {
        public AirportSpecs AirportInformation { get; set; }
        public WeatherData LastFetchedWeatherData { get; set; }
        public IEnumerable<ParkingLotViewDto> ParkingLotView { get; set; }
        public  IEnumerable<StateChangeHistoryDto> Last10AirlineStateChangeHistory { get; set; }
    }
}
