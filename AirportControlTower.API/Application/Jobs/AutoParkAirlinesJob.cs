using AirportControlTower.API.Application.Commands;
using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace AirportControlTower.API.Application.Jobs
{
    public class AutoParkAirlinesJob(AppDbContext dbContext,
        IMediator mediator,
        ILogger<AutoParkAirlinesJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var landedAirlines = await dbContext.Airlines
                .Where(a => a.State == AirlineState.Landed)
                .ToListAsync();

            foreach (var airline in landedAirlines)
            {
                try
                {
                    await mediator.Send(new RequestStateChangeCommand
                    {
                        CallSign = airline.CallSign,
                        State = AirlineStateTrigger.Park
                    });

                    logger.LogInformation("Auto-parked airline {callSign}", airline.CallSign);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to auto-park airline {callSign}", airline.CallSign);
                }
            }
        }
    }
}
