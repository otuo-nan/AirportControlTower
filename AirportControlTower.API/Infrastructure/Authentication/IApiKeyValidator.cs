namespace AirportControlTower.API.Infrastructure.Authentication
{
    public interface IApiKeyValidator
    {
        Task<bool> IsValidApiKeyAsync(string apiKey);
    }

    public class ApiKeyValidator(IConfiguration configuration) : IApiKeyValidator
    {
        public Task<bool> IsValidApiKeyAsync(string apiKey)
        {
            // In a real application, you might want to check against a database
            // or other storage mechanism. This is a simple example using configuration.
            var validApiKey = configuration["ApiKey"];
            return Task.FromResult(apiKey == validApiKey);
        }
    }
}