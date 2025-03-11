using AirportControlTower.API.Models;

#nullable disable
namespace AirportControlTower.API.Application.Dtos
{
    public class ListAirlineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public AirlineState State { get; set; }
        public AirlineType Type { get; set; }
        public string CallSign { get; set; }
        public Position LastKnownPosition { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class ParkingLotViewDto
    {
        public AirlineType AirlineType { get; set; }
        public IEnumerable<ParkedAirlineDto> Airlines { get; set; }
    }


    public class ParkedAirlineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CallSign { get; set; }
        public AirlineType Type { get; set; }
    }
}
