using System.Text.Json.Serialization;

namespace AirportControlTower.Dashboard.Dtos
{
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
}
