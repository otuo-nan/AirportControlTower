
#nullable disable
namespace AirportControlTower.Dashboard.Dtos
{
    public class DashboardDto
    {
        public AirportSpecs AirportInformation { get; set; }
        public WeatherData LastFetchedWeatherData { get; set; }
        public IEnumerable<ParkingLotViewDto> ParkingLotView { get; set; }
        public  IEnumerable<StateChangeHistoryDto> Last10AirlineStateChangeHistory { get; set; }
    }

    public class AirportSpecs
    {
        public string Location { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public int Runways { get; set; }
        public int AirlineParkingSlots { get; set; }
        public int PrivateParkingSlots { get; set; }
    }
}
