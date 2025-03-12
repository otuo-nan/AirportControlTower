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
        IOptions<OpenWeatherApi> openWeatherApiOptions,
        IOptions<AirportSpecs> airPortConfigOptions,
        ILogger<WeatherService> logger)
    {
        readonly AirportSpecs airportSpecs = airPortConfigOptions.Value;
        readonly OpenWeatherApi openWeatherApiConfig = openWeatherApiOptions.Value;

        public async Task<WeatherData?> GetWeatherInformationAsync(string? callSign = default)
        {
            //var airline = await GetAirlineAsync(callSign);

            //find local first, if doesnt exist, call the api
            var weather = await GetWeatherAsync();

            if (weather != null)
            {
                return weather.Data;
            }
            else
            {
                var openWeather = await GetWeatherInformationAsync(airportSpecs.Lat, airportSpecs.Lon);
                if (openWeather != null)
                {
                    return await CreateWeatherAsync(openWeather);
                }

                return default;
            }
        }

        /// <summary>
        /// Used by background service
        /// </summary>
        /// <returns></returns>
        public async Task UpdateLocalWeatherStoreAsync()
        {
            Weather? weather = await dbContext.Weather.SingleOrDefaultAsync();

            var openWeather = await GetWeatherInformationAsync(airportSpecs.Lat, airportSpecs.Lon);

            if (openWeather != null)
            {
                if (weather == null)
                {
                    _ = await CreateWeatherAsync(openWeather);
                }
                else
                {
                    await UpdateWeatherAsync(weather.Id, openWeather);
                }
            }
        }

        #region helpers
        async Task<Airline> GetAirlineAsync(string callSign)
        {
            return await dbContext.Airlines.FirstAsync(a => a.CallSign == callSign);
        }

        async Task<Weather?> GetWeatherAsync()
        {
            return await dbContext.Weather.SingleOrDefaultAsync();
        }

        async Task<WeatherInformation?> GetWeatherInformationAsync(float lat, float lon)
        {
            try
            {
                var response = await httpClient.GetAsync($"weather?lat={lat}&lon={lon}&appid={openWeatherApiConfig.ApiKey}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStreamAsync();
                return JsonSerializer.Deserialize<WeatherInformation>(content, Utility.SerializerOptions)!;
            }
            catch (Exception ex)
            {
                logger.LogError("Weather unavailable. \n\n{message}", ex.Message);
                return default;
            }
        }

        async Task<WeatherData> CreateWeatherAsync(WeatherInformation openWeather)
        {
            var weather = new Weather
            {
                Data = MapOpenWeatherToWeatherData(openWeather),
                LastUpdate = DateTime.UtcNow,
            };

            await dbContext.Weather.AddAsync(weather);
            await dbContext.SaveChangesAsync();

            return weather.Data;
        }

        async Task UpdateWeatherAsync(Guid id, WeatherInformation openWeather)
        {
            await dbContext.Weather.Where(w => w.Id == id).ExecuteUpdateAsync(setter =>
                            setter.SetProperty(p => p.Data, MapOpenWeatherToWeatherData(openWeather))
                                  .SetProperty(p => p.LastUpdate, DateTime.UtcNow));
        }

        static WeatherData MapOpenWeatherToWeatherData(WeatherInformation openWeather)
        {
            return new WeatherData
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
            };
        }
        #endregion helpers
    }
}
