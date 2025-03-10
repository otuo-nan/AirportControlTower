namespace AirportControlTower.API.Infrastructure.Configurations
{
    public class AirportSpecs
    {
        public string Location { get; set; } = default!;
        public int Runways { get; set; }
        public int AirlineParkingSlots { get; set; }
        public int PrivateParkingSlots { get; set; }
    }
}
