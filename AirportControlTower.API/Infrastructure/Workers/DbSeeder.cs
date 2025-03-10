using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace AirportControlTower.API.Infrastructure.Workers
{
    public class DbSeeder(IServiceProvider serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

            if (!await dbContext.Airlines.AnyAsync(cancellationToken))
            {
                await dbContext.Airlines.AddRangeAsync(GetEntitites<Airline[]>("airlines.json"));

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        static T GetEntitites<T>(string fileLocation) where T : class
        {
            var data = File.ReadAllText(GetFilePath(fileLocation));
            return JsonSerializer.Deserialize<T>(data, Utility.SerializerOptions)!;
        }

        static string GetFilePath(string fileName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(Directory.GetCurrentDirectory(), @$"Infrastructure/Workers/Files/{fileName}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Directory.GetCurrentDirectory(), @$"Infrastructure\Workers\Files\{fileName}");
            }

            return string.Empty;
        }
    }
}
