using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AirportControlTower.API.Application.Queries
{
    public class WeatherQueries(AppDbContext dbContext)
    {
        public async Task<WeatherData?> GetLastFetchedWeatherAsync()
        {
            return (await dbContext.Weather.OrderByDescending(w => w.LastUpdate).FirstOrDefaultAsync())?.Data;
        }
    }
}
