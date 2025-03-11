
#nullable disable
namespace AirportControlTower.Dashboard.Dtos
{
    public class DashboardDto
    {
        public WeatherData LastFetchedWeatherData { get; set; }
        public IEnumerable<ParkingLotViewDto> ParkingLotView { get; set; }
        public  IEnumerable<StateChangeHistoryDto> Last10AirlineStateChangeHistory { get; set; }
    }
}
