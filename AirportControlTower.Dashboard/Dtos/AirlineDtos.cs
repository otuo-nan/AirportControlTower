
#nullable disable
using System.Text.Json.Serialization;

namespace AirportControlTower.Dashboard.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AirlineState : byte { Parked = 1, TakingOff = 2, Airborne = 3, Approach = 4, Landed = 5 }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AirlineStateTrigger : byte { Park = 1, TakeOff = 2, IsAirborne = 3, Approach = 4, Land = 6, Abort = 7 }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AirlineType : byte { Airliner = 1, Private = 2 }

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

    public class Position
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public float Heading { get; set; }
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
