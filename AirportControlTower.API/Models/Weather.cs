using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AirportControlTower.API.Models
{
    public class Weather
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(Airline))]
        public Guid AirlineId { get; set; }
        public Airline Airline { get; set; } = default!;
        public WeatherData Data { get; set; } = default!;
        public DateTime LastUpdate { get; set; }
    }

#nullable disable
    public class WeatherData
    {
        public string Description { get; set; }
        public int Temperature { get; set; }
        public int Visibility { get; set; }
        public Wind Wind { get; set; }

        [JsonPropertyName("last_update")]
        public DateTime LastUpdate { get; set; }
    }
    public class Wind
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
    }
#nullable enable
}
