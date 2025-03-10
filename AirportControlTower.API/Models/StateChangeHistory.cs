using AirportControlTower.API.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AirportControlTower.API.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HistoryStatus : byte { Accepted = 1, Rejected = 2 }

    public class StateChangeHistory
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(Airline))]
        public Guid AirlineId { get; set; }
        public Airline Airline { get; set; } = default!;
        public HistoryStatus Status { get; set; }
        public DateTime CreatedOn { get; set; } = Utility.GetDateTimeNow;

    }
}
