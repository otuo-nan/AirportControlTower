using AirportControlTower.API.Application.Dtos;
using AirportControlTower.API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AirportControlTower.API.Application.Queries
{
    public class StateChangeHistoryQueries(AppDbContext dbContext)
    {
        public async Task<(IList<StateChangeHistoryDto> entities, int count)> GetHistoryAsync(int page, int pageSize, string? searchQuery, string? orderBy, string orderDirection = "ASC")
        {
            orderBy = string.IsNullOrEmpty(orderBy) ? nameof(StateChangeHistoryDto.CreatedOn) : orderBy;

            var entities = dbContext.StateChangeHistory.Include(s => s.Airline).AsNoTracking()
                .Select(a => new StateChangeHistoryDto
                {
                    Id = a.Id,
                    AirlineId = a.AirlineId,
                    AirlineCallSign = a.Airline.CallSign,
                    AirlineName = a.Airline.Name,
                    AirlineType = a.Airline.Type,
                    FromState = a.FromState,
                    Trigger = a.Trigger,
                    Status = a.Status,
                    CreatedOn = a.CreatedOn
                });

            if (!string.IsNullOrEmpty(searchQuery))
            {
                entities = entities.Where(p => p.AirlineName.Contains(searchQuery) || p.AirlineName.Contains(searchQuery));
            }

            var count = await entities.CountAsync();

            entities = entities.SortByDynamic(orderBy, orderDirection)
                               .Page(page, pageSize);

            return (await entities.ToListAsync(), count);
        }

        public async Task<IEnumerable<StateChangeHistoryDto>> Last_N_AirlineStateChangeHistoryAsync(int n)
        {
            return await dbContext.StateChangeHistory.AsNoTracking()
                .OrderByDescending(s => s.CreatedOn)
                .Take(n)
                .Select(a => new StateChangeHistoryDto
                {
                    Id = a.Id,
                    AirlineId = a.AirlineId,
                    AirlineCallSign = a.Airline.CallSign,
                    AirlineName = a.Airline.Name,
                    AirlineType = a.Airline.Type,
                    FromState = a.FromState,
                    Trigger = a.Trigger,
                    Status = a.Status,
                    CreatedOn = a.CreatedOn
                })
                .ToListAsync();
        }
    }
}
