using Microsoft.EntityFrameworkCore;
using AirportControlTower.Dashboard.Database;
using Microsoft.AspNetCore.Identity;

namespace AirportControlTower.Dashboard;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }


        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}
