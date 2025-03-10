using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AirportControlTower.API.Models;
using System.Text.Json;
using AirportControlTower.API.Application.Dtos;

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
    
    public class WeatherConfiguration : IEntityTypeConfiguration<Weather>
    {
        public void Configure(EntityTypeBuilder<Weather> entity)
        {
            entity.Property(p => p.Data)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, Utility.SerializerOptions),
                      v => JsonSerializer.Deserialize<WeatherData>(v, Utility.SerializerOptions)!
                  );
        }
    }
}
