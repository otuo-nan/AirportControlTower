using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AirportControlTower.Dashboard.Utility
{
    public static class IdentityExtensions
    {
        public static async Task<string> GetEmail(this Task<AuthenticationState> authenticationState)
        {
            return (await authenticationState).User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        }
    }
}
