using AirportControlTower.Dashboard.Data;
using AirportControlTower.Dashboard.Dtos;
using System.Threading;

namespace AirportControlTower.Dashboard.Services
{
    public class DashboardService(HttpClient httpClient, ILogger<DashboardService> logger)
    {
        public async Task<PaginatedEntities<ListAirlineDto>?> AirlinesThatHaveMadeContactToControlTowerAsync(
            PagingOptionsDto options,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var httpParams = $"admin/airline-contacts?page={options.Page}&pageSize={options.PageSize}&searchQuery={options.SearchQuery}" +
                    $"&sortfield={options.CurrentSortField}&sortDirection={options.CurrentSortDirection}";
                return await httpClient.GetFromJsonAsync<PaginatedEntities<ListAirlineDto>>(httpParams, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        } 
        
        public async Task<PaginatedEntities<StateChangeHistoryDto>?> AirlineStateChangeHistoryAsync(
            PagingOptionsDto options, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var httpParams = $"admin/airline-state-change-reqest-history?page={options.Page}&pageSize={options.PageSize}&searchQuery={options.SearchQuery}" +
                    $"&sortfield={options.CurrentSortField}&sortDirection={options.CurrentSortDirection}";
                return await httpClient.GetFromJsonAsync<PaginatedEntities<StateChangeHistoryDto>>(httpParams, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }
        
        public async Task<DashboardDto?> GetDashboardDataAsync()
        {
            try
            {
                var httpParams = $"admin/dashboard";
                return await httpClient.GetFromJsonAsync<DashboardDto>(httpParams);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        } 
        
        public async Task<IEnumerable<StateChangeHistoryDto>?> MostRecentAirlineStateChangeHistory()
        {
            try
            {
                var httpParams = $"admin/last-n-airline-state-change-reqest-history?n=10";
                return await httpClient.GetFromJsonAsync<IEnumerable<StateChangeHistoryDto>>(httpParams);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
