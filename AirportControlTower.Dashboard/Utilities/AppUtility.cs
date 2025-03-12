using Microsoft.JSInterop;
using System.Text.Json;

namespace AirportControlTower.Dashboard.Utilities
{
    public static class Utility
    {
        public static DateTime GetDateTimeNow => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        public static JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
        };

        public static bool IsDocker(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment("Docker");
        }

        public static async Task ShowModalAsync(this IJSRuntime JS, string modalId = "details-modal")
        {
            await JS.InvokeVoidAsync("bootstrapInteropt.showModal", modalId);
        }

        public static async Task HideModalAsync(this IJSRuntime JS, string modalId = "details-modal")
        {
            await JS.InvokeVoidAsync("bootstrapInteropt.hideModal", modalId);
        }

        static public TValue GetObjectPropertyValue<TValue>(this object obj, string propertyName)
        {
            return (TValue)obj.GetType().GetProperty(propertyName)!.GetValue(obj)!;
        }
    }
}
