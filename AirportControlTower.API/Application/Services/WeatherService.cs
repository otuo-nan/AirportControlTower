using AirportControlTower.API.Infrastructure;
using AirportControlTower.API.Infrastructure.Configurations;
using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static AirportControlTower.API.Application.Dtos.OpenWeatherDtos;

namespace AirportControlTower.API.Application.Services
{
    public class WeatherService(HttpClient httpClient,
        AppDbContext dbContext,
        IOptions<OpenWeatherApi> options)
    {
        readonly OpenWeatherApi openWeatherApiConfig = options.Value;
        public async Task<WeatherData> GetWeatherInformationAsync(string callSign)
        {
            var airline = await GetAirlineAsync(callSign);

            //find local first, if doesnt exist, call the api
            var weather = await GetAirlineWeatherAsync(airline.Id);

            if (weather != null)
            {
                return weather.Data;
            }
            else
            {
                var openWeather = await GetWeatherInformationAsync(airline.LastKnownPosition);

                return await SaveWeatherInformation(airline.Id, openWeather);
            }
        }

        /// <summary>
        /// Used by background service
        /// </summary>
        /// <returns></returns>
        public async Task UpdateLocalWeatherStoreAsync()
        {
            var weatherQueriedByAirlines = await dbContext.Weather.ToListAsync();

            foreach (var weather in weatherQueriedByAirlines)
            {
                var airline = await GetAirlineAsync(weather.AirlineId);
                var openWeather = await GetWeatherInformationAsync(airline.LastKnownPosition);

                await SaveWeatherInformation(airline.Id, openWeather);
            }
        }


        #region helpers
        async Task<Airline> GetAirlineAsync(string callSign)
        {
            return await dbContext.Airlines.FirstAsync(a => a.CallSign == callSign);
        }

        async Task<Airline> GetAirlineAsync(Guid id)
        {
            return await dbContext.Airlines.FirstAsync(a => a.Id == id);
        }
        
        async Task<Weather?> GetAirlineWeatherAsync(Guid airlineId)
        {
            return await dbContext.Weather.FirstOrDefaultAsync(a => a.AirlineId == airlineId);
        }

        async Task<WeatherInformation> GetWeatherInformationAsync(Position position)
        {
            var response = await httpClient.GetAsync($"weather?lat={position.Latitude}&lon={position.Longitude}&appid={openWeatherApiConfig.ApiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<WeatherInformation>(content, Utility.SerializerOptions)!;
        }

        async Task<WeatherData> SaveWeatherInformation(Guid airlineId, WeatherInformation openWeather)
        {
            var weather = new Weather
            {
                AirlineId = airlineId,
                Data = new WeatherData
                {
                    Description = openWeather.Weather.First().Description,
                    Temperature = (int)openWeather.Main.Temp,
                    Visibility = openWeather.Visibility,
                    Wind = new Models.Wind
                    {
                        Speed = openWeather.Wind.Speed,
                        Deg = openWeather.Wind.Deg
                    },
                    LastUpdate = Utility.GetDateTimeNow
                },
                LastUpdate = DateTime.UtcNow
            };

            await dbContext.Weather.AddAsync(weather);
            await dbContext.SaveChangesAsync();

            return weather.Data;
        }

        #endregion helpers
    }
}
