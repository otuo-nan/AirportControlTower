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
        IOptions<AirportSpecs> airPortConfigOptions)
    {
        readonly AirportSpecs airportSpecs = airPortConfigOptions.Value;
        readonly OpenWeatherApi openWeatherApiConfig = openWeatherApiOptions.Value;

        public async Task<WeatherData> GetWeatherInformationAsync(string callSign)
        {
            var airline = await GetAirlineAsync(callSign);

            //find local first, if doesnt exist, call the api
            var weather = await GetWeatherAsync();

            if (weather != null)
            {
                return weather.Data;
            }
            else
            {
                var openWeather = await GetWeatherInformationAsync(airportSpecs.Lat, airportSpecs.Lon);

                return await CreateWeatherAsync(airline.Id, openWeather);
            }
        }

        /// <summary>
        /// Used by background service
        /// </summary>
        /// <returns></returns>
        public async Task UpdateLocalWeatherStoreAsync()
        {
            var weather = await dbContext.Weather.SingleAsync();
            var openWeather = await GetWeatherInformationAsync(airportSpecs.Lat, airportSpecs.Lon);

            await UpdateWeatherAsync(weather.Id, openWeather);
        }


        #region
        //ToDo: should be just one record?
        public async Task<WeatherData> GetWeatherInformationAsyncv1(string callSign)
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
                var openWeather = await GetWeatherInformationAsync(airportSpecs.Lat, airportSpecs.Lon);

                return await CreateWeatherAsync(airline.Id, openWeather);
            }
        }

        /// <summary>
        /// Used by background service
        /// </summary>
        /// <returns></returns>
        //ToDo: should be just one record?
        public async Task UpdateLocalWeatherStoreAsyncv1()
        {
            var weatherQueriedByAirlines = await dbContext.Weather.ToListAsync();

            foreach (var weather in weatherQueriedByAirlines)
            {
                var airline = await GetAirlineAsync(weather.AirlineId);
                var openWeather = await GetWeatherInformationAsync(airportSpecs.Lat, airportSpecs.Lon);

                await UpdateWeatherAsync(airline.Id, openWeather);
            }
        }
        #endregion

        #region helpers
        async Task<Airline> GetAirlineAsync(string callSign)
        {
            return await dbContext.Airlines.FirstAsync(a => a.CallSign == callSign);
        }

        async Task<Airline> GetAirlineAsync(Guid id)
        {
            return await dbContext.Airlines.FirstAsync(a => a.Id == id);
        }

        async Task<Weather?> GetWeatherAsync()
        {
            return await dbContext.Weather.SingleOrDefaultAsync();
        }

        async Task<Weather?> GetAirlineWeatherAsync(Guid airlineId)
        {
            return await dbContext.Weather.FirstOrDefaultAsync(a => a.AirlineId == airlineId);
        }

        async Task<WeatherInformation> GetWeatherInformationAsync(float lat, float lon)
        {
            var response = await httpClient.GetAsync($"weather?lat={lat}&lon={lon}&appid={openWeatherApiConfig.ApiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<WeatherInformation>(content, Utility.SerializerOptions)!;
        }

        async Task<WeatherData> CreateWeatherAsync(Guid airlineId, WeatherInformation openWeather)
        {
            var weather = new Weather
            {
                AirlineId = airlineId,
                Data = MapOpenWeatherToWeatherData(openWeather),
                LastUpdate = DateTime.UtcNow,
            };

            await dbContext.Weather.AddAsync(weather);
            await dbContext.SaveChangesAsync();

            return weather.Data;
        }

        async Task UpdateWeatherAsync(Guid airlineId, WeatherInformation openWeather)
        {
            await dbContext.Weather.Where(w => w.AirlineId == airlineId).ExecuteUpdateAsync(setter =>
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
