using Microsoft.AspNetCore.Identity;

#nullable disable
namespace AirportControlTower.API.Infrastructure.Workers
{
    public class UserSeeder
    {
        public static async Task LoadUsers(IServiceProvider service, List<UserDto> users)
        {
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var u in users)
            {
                IdentityUser user = new()
                {
                    Id = u.Id,
                    UserName = u.Email,
                    Email = u.Email,
                };

                var result = await userManager.CreateAsync(user, u.Password);

                if (!result.Succeeded)
                {
                    throw new PlatformException([.. result.Errors.Select(err => err.Description)]);
                }
            }
        }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public Guid? BusinessId { get; set; }
        public string Password { get; set; } = default!;
    }
}
