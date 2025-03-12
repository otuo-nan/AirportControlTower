using AirportControlTower.Dashboard.Dtos;
using AirportControlTower.Dashboard.Services;
using Microsoft.AspNetCore.Components;

namespace AirportControlTower.Dashboard.Pages
{
    public partial class Dashboard
    {
        [Inject]
        public ILogger<AirlineContacts> Logger { get; set; } = null!;


        [Inject]
        public DashboardService Service { get; set; } = default!;

        DashboardDto Model = default!;

        protected override async Task OnInitializedAsync()
        {
           Model = (await Service.GetDashboardDataAsync())!;
        }

        public static string FormatCoordinates(double latitude, double longitude)
        {
            string latDirection = latitude >= 0 ? "N" : "S";
            string lonDirection = longitude >= 0 ? "E" : "W";

            return $"{Math.Abs(latitude):F6}°{latDirection}, {Math.Abs(longitude):F6}°{lonDirection}";
        }
    }
}
