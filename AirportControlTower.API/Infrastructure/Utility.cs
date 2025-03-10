using System.Security.Claims;
using System.Text.Json;

namespace AirportControlTower.API.Infrastructure
{
    public static class Utility
    {
        public static DateTime GetDateTimeNow => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        public static JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
        };

        public static bool IsEnvironmentDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        public static bool IsEnvironmentProduction => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        public static string GetValuefromClaim(this IEnumerable<Claim> claims, string name) => claims.First(c => c.Type == name).Value;
    }
}
