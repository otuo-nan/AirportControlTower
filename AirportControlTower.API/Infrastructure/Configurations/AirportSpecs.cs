namespace AirportControlTower.API.Infrastructure.Configurations
{
#nullable disable
    public class AirportSpecs
    {
        public string Location { get; set; }
        public int Runways { get; set; }
        public int AirlineParkingSlots { get; set; }
        public int PrivateParkingSlots { get; set; }
    }

    public class OpenWeatherApi
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }
}
