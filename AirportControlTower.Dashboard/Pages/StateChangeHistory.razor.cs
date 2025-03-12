using AirportControlTower.Dashboard.Data;
using AirportControlTower.Dashboard.Dtos;
using AirportControlTower.Dashboard.Services;
using AirportControlTower.Dashboard.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AirportControlTower.Dashboard.Pages
{
    public partial class StateChangeHistory
    {
        #region fields + props
        IEnumerable<StateChangeHistoryDto> listEntities = [];
        bool _isProcessing;
        StateChangeHistoryDto? _airlineDetails;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Inject]
        public ILogger<AirlineContacts> Logger { get; set; } = null!;


        [Inject]
        public DashboardService Service { get; set; } = default!;

        readonly PagingOptionsDto pagingOptions = new()
        {
            CurrentSortField = nameof(StateChangeHistoryDto.CreatedOn),
            CurrentSortDirection = "Desc"
        };

        protected PageMetaData pageMetaData = new();

        private string _searchQuery = string.Empty;
        private CancellationTokenSource currentSearchCts = null!;

        string SearchQuery
        {
            get => _searchQuery;

            set
            {
                _searchQuery = value;
                _ = SearchDebouncedAsync();
            }
        }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await GetEntitiesAsync();
        }

        async Task SearchDebouncedAsync()
        {
            try
            {
                currentSearchCts?.Cancel();
                currentSearchCts = new CancellationTokenSource();
                var thisSearchToken = currentSearchCts.Token;

                await Task.Delay(500);

                if (!thisSearchToken.IsCancellationRequested)
                {
                    pageMetaData.Reset();
                    await GetEntitiesAsync(thisSearchToken);
                }

                StateHasChanged();
            }
            catch (System.Exception ex)
            {
                Logger.LogInformation(ex.StackTrace);
            }
        }

        async Task GetEntitiesAsync(CancellationToken cancellationToken = default)
        {
            _isProcessing = true;

            SetMorePagingOptions();
            PaginatedEntities<StateChangeHistoryDto>? paged = await Service.AirlineStateChangeHistoryAsync(pagingOptions, cancellationToken);

            if (paged != null)
            {
                listEntities = paged.Data;
                SetPageMetadataFromPagedEntities(paged.PagingOptions);
            }

            _isProcessing = false;
        }

        void SetMorePagingOptions()
        {
            pagingOptions.SearchQuery = SearchQuery;
            pagingOptions.Page = pageMetaData.CurrentPage;
            pagingOptions.PageSize = pageMetaData.PageSize;
        }

        void SetPageMetadataFromPagedEntities(PagingOptions pagingOptions)
        {
            pageMetaData.TotalPages = pagingOptions.NumPages;
            pageMetaData.PageSize = pagingOptions.PageSize;
            pageMetaData.TotalCount = pagingOptions.TotalCount;
        }

        async Task ListRow_DbClicked(StateChangeHistoryDto dto)
        {
            _airlineDetails = dto;
            await JS.ShowModalAsync();
        }


        string GetLastKnownPosition(Position position)
        {
            return $"Lat-{position.Latitude}, Lon-{position.Longitude}";
        }

        static string GetStateStatusCss(AirlineState status)
        {
            return status switch
            {
                AirlineState.Parked => "text-info",
                AirlineState.TakingOff => "text-danger",
                AirlineState.Airborne => "text-success",
                AirlineState.Approach => "text-danger",
                AirlineState.Landed => "text-primary",
                _ => string.Empty,
            };
        } 
        
        static string GetHistoryStatusCss(HistoryStatus status)
        {
            return status switch
            {
                HistoryStatus.Accepted => "text-success",
                HistoryStatus.Rejected => "text-danger",
                _ => string.Empty,
            };
        }

        #region Paging
        async Task Sort(string sortField)
        {
            if (sortField.Equals(pagingOptions.CurrentSortField))
            {
                pagingOptions.CurrentSortDirection =
                    pagingOptions.CurrentSortDirection.Equals("Asc") ? "Desc" : "Asc";
            }
            else
            {
                pagingOptions.CurrentSortField = sortField;
                pagingOptions.CurrentSortDirection = "Asc";
            }

            await GetEntitiesAsync();
        }

        string SortIndicator(string sortField)
        {
            if (sortField.Equals(pagingOptions.CurrentSortField))
            {
                return pagingOptions.CurrentSortDirection.Equals("Asc") ? "fas fa-sort-down" : "fas fa-sort-up";
            }

            return "fas fa-sort-down";
        }

        protected async Task SelectedPage_Callback()
        {
            await GetEntitiesAsync();
        }
    }
    #endregion
}
