using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AirportControlTower.API.Infrastructure.Authentication
{
    public class ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder,
        IApiKeyValidator apiKeyValidator) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        private const string ApiKeyHeaderName = "X-API-Key";
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.Fail("API Key was not provided.");
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrEmpty(providedApiKey))
            {
                return AuthenticateResult.Fail("API Key value was not provided.");
            }

            if (!await apiKeyValidator.IsValidApiKeyAsync(providedApiKey))
            {
                return AuthenticateResult.Fail("Invalid API Key.");
            }

            var claims = new[] {
            new Claim(ClaimTypes.Name, "API User"),
            new Claim(ClaimTypes.Role, "ApiUser")
                 };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }

    public class ApiKeyAuthorizeAttribute : AuthorizeAttribute
    {
        public ApiKeyAuthorizeAttribute()
        {
            AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme;
        }
    }

    public class ApiKeyAuthenticationDefaults 
    {
        public const string AuthenticationScheme = "ApiKey";
    }

}
