using AirportControlTower.API.Application.Services;
using AirportControlTower.API.Infrastructure;
using Quartz;

namespace AirportControlTower.API.Application.Jobs
{
    [DisallowConcurrentExecution]
    public class WeatherUpdateJob(WeatherService weatherService, ILogger<WeatherUpdateJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                logger.LogInformation("Starting weather update job at: {time}", DateTime.UtcNow);

                await weatherService.UpdateLocalWeatherStoreAsync();
                logger.LogInformation("Completed weather update job at: {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating weather information");
                throw;
            }
        }
    }
}
