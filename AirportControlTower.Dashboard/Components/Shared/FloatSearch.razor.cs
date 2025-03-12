using AirportControlTower.Dashboard.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AirportControlTower.Dashboard.Components.Shared
{
    public partial class FloatSearch
    {
        string _searchQuery = string.Empty;
        bool isProcessing = false;

        CancellationTokenSource currentSearchCts = null!;

        IEnumerable<object> listEntities = [];

        [Parameter]
        public string TopDivClassName { get; set; } = default!;

        [Parameter]
        public EventCallback<object> ItemSelected_CallBack { get; set; }

        [Parameter]
        public Func<string, Task<IEnumerable<object>>> SearchDelegate { get; set; } = default!;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Inject]
        public ILogger<FloatSearch> Logger { get; set; } = null!;

        string SearchQuery
        {
            get => _searchQuery;

            set
            {
                _searchQuery = value;
                _ = SearchDebouncedAsync();
            }
        }

        async Task SearchDebouncedAsync()
        {
            isProcessing = true;

            try
            {
                currentSearchCts?.Cancel();
                currentSearchCts = new CancellationTokenSource();
                var thisSearchToken = currentSearchCts.Token;

                await Task.Delay(500);

                if (!thisSearchToken.IsCancellationRequested)
                {
                    await GetEntitiesAsync();
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.StackTrace);
            }

            isProcessing = false;
        }

        async Task GetEntitiesAsync()
        {
            isProcessing = true;
            listEntities = await SearchDelegate.Invoke(SearchQuery);
            isProcessing = false;
        }

        static (string meta1, string meta2) GetMetadata(object dto)
        {
            var meta1 = dto.GetObjectPropertyValue<string>("Metadata");
            var meta2 = dto.GetObjectPropertyValue<string>("Metadata2");

            return (meta1, meta2);
            //var meta2 = type.GetProperty("Metadata2", typeof(string))!.GetValue(dto) as string;
        }

        async Task ItemSelected(object item)
        {
            listEntities = Enumerable.Empty<object>();
            await ItemSelected_CallBack.InvokeAsync(item);
            SearchQuery = string.Empty;
        }

        string SearchResultsCss() => listEntities.Any() ? "search_results" : string.Empty;
    }
}
