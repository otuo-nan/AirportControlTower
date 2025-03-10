using AirportControlTower.API.Application.Services;
using AirportControlTower.API.Infrastructure;
using AirportControlTower.API.Infrastructure.Authentication;
using AirportControlTower.API.Infrastructure.Configurations;
using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Infrastructure.Workers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;

namespace AirportControlTower.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(configure =>
            {
                configure.Filters.Add<HttpGlobalExceptionFilter>();
            });

            builder.Services.AddScoped<IApiKeyValidator, ApiKeyValidator>();

            builder.Services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationDefaults.AuthenticationScheme, null);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

            builder.Services.Configure<AirportSpecs>(builder.Configuration.GetSection("AirportSpecs"));
            builder.Services.Configure<OpenWeatherApi>(builder.Configuration.GetSection("OpenWeatherApi"));

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>() // This line requires the Microsoft.AspNetCore.Identity.UI namespace
                          .AddEntityFrameworkStores<AppDbContext>()
                          .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            });

            builder.Services.AddHttpClient<WeatherService>(options =>
            {
                options.BaseAddress = new Uri(builder.Configuration["OpenWeatherApi:BaseUrl"]!);
            });

            builder.Services.AddQuartzJobs();
            builder.Services.AddHostedService<DbSeeder>();

            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "AirportControlTower.API", Version = "v1" });
                option.AddSecurityDefinition("API Key", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid key",
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey,
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "API Key"
                            }
                        },
                        Array.Empty<string>()
                    }
               });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}