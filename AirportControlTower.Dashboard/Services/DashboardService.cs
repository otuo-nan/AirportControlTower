using AirportControlTower.Dashboard.Data;
using AirportControlTower.Dashboard.Dtos;

namespace AirportControlTower.Dashboard.Services
{
    public class DashboardService(HttpClient httpClient, ILogger<DashboardService> logger)
    {
        public async Task<PaginatedEntities<ListAirlineDto>?> AirlinesThatHaveMadeContactToControlTowerAsync(PagingOptionsDto options)
        {
            try
            {
                var httpParams = $"admin/airline-contacts?page={options.Page}&pageSize={options.PageSize}&sortfield={options.CurrentSortField}&sortDirection={options.CurrentSortDirection}";
                return await httpClient.GetFromJsonAsync<PaginatedEntities<ListAirlineDto>>(httpParams);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        } 
        
        public async Task<PaginatedEntities<StateChangeHistoryDto>?> AirlineStateChangeHistoryAsync(PagingOptionsDto options)
        {
            try
            {
                var httpParams = $"admin/airline-state-change-reqest-history?page={options.Page}&pageSize={options.PageSize}&sortfield={options.CurrentSortField}&sortDirection={options.CurrentSortDirection}";
                return await httpClient.GetFromJsonAsync<PaginatedEntities<StateChangeHistoryDto>>(httpParams);
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
    }
}
