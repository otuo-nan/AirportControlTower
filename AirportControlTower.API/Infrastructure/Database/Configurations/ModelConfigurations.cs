using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AirportControlTower.API.Models;
using System.Text.Json;

namespace AirportControlTower.API.Infrastructure.Database.Configurations
{
    public class AirlineConfiguration : IEntityTypeConfiguration<Airline>
    {
        public void Configure(EntityTypeBuilder<Airline> entity)
        {
            entity.Property(p => p.LastKnownPosition)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, Utility.SerializerOptions),
                      v => JsonSerializer.Deserialize<Position>(v, Utility.SerializerOptions)!
                  );
        }
    }
}
