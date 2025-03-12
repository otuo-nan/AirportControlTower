using AirportControlTower.API.Application.Dtos;
using AirportControlTower.API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AirportControlTower.API.Application.Queries
{
    public class AirlineQueries(AppDbContext dbContext)
    {
        public async Task<(IList<ListAirlineDto> entities, int count)> GetAirlinesAsync(int page, int pageSize, string? searchQuery, string? orderBy, string orderDirection = "ASC")
        {
            orderBy = string.IsNullOrEmpty(orderBy) ? nameof(ListAirlineDto.LastUpdate) : orderBy;

            var entities = dbContext.Airlines.AsNoTracking()
                .Select(a => new ListAirlineDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    State = a.State,
                    Type = a.Type,
                    CallSign = a.CallSign,
                    LastKnownPosition = a.LastKnownPosition,
                    LastUpdate = a.LastUpdate
                });

            if (!string.IsNullOrEmpty(searchQuery))
            {
                entities = entities.Where(p => p.Name.Contains(searchQuery) || p.CallSign.Contains(searchQuery));
            }

            var count = await entities.CountAsync();

            entities = entities.SortByDynamic(orderBy, orderDirection)
                               .Page(page, pageSize);

            return (await entities.ToListAsync(), count);
        }

        public async Task<IEnumerable<ParkingLotViewDto>> GetParkingLotViewAsync()
        {
            return await dbContext.Airlines.AsNoTracking()
                                .Where(a => a.State == Models.AirlineState.Parked)
                                .GroupBy(a => a.Type)
                                .Select(g => new ParkingLotViewDto
                                {
                                    AirlineType = g.Key,
                                    Airlines = g.Select(a => new ParkedAirlineDto
                                    {
                                        Id = a.Id,
                                        Name = a.Name,
                                        CallSign = a.CallSign,
                                        Type = a.Type,

                                    }).ToList()
                                })
                                .ToListAsync();
        }
    }
}
