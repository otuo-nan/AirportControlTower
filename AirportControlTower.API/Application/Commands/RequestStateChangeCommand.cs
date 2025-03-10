using AirportControlTower.API.Application.FSM;
using AirportControlTower.API.Infrastructure;
using AirportControlTower.API.Infrastructure.Configurations;
using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace AirportControlTower.API.Application.Commands
{
    public class RequestStateChangeCommand : IRequest
    {
        public string CallSign { get; set; } = default!;
        public AirlineStateTrigger State { get; set; }
    }

    public class RequestChangeCommandHandler(AppDbContext dbContext,
        ILogger<RequestChangeCommandHandler> logger,
        IOptions<AirportSpecs> airportSpecsOptions) : IRequestHandler<RequestStateChangeCommand>
    {
        readonly AirportSpecs airportSpecs = airportSpecsOptions.Value;
        Airline airline = default!;
        public async Task Handle(RequestStateChangeCommand command, CancellationToken cancellationToken)
        {
            airline = await GetAirlineAsync(command.CallSign);

            var fsm = new AirlineStateMachine(airline, await VerifyRunwayAvailability(), await VerifyRunwayApproachability());
            fsm.StateChanged += StateChanged;
            fsm.StateChangedFailed += StateChangedFailed;

            fsm.Fire(command.State);


            void StateChangedFailed(object? sender, TransitionChangedFailedEventArgs e)
            {
                dbContext.StateChangeHistory.Add(CreateHistory(HistoryStatus.Rejected));
                dbContext.SaveChanges();
                logger.LogError("Failed to change state of airline {callSign} from {state1} to {state2}", airline.CallSign, airline.State, command.State);
                throw new PlatformException { CustomStatusCode = (int)HttpStatusCode.Conflict };
            }
        }

        async Task<Airline> GetAirlineAsync(string callSign)
        {
            return await dbContext.Airlines.FirstAsync(a => a.CallSign == callSign);
        }

        async Task<bool> VerifyRunwayAvailability()
        {
            //check if there are any aircrafts on the runway
            //check if parking lots are available for this type of aircraft
            return await IsParkingLotAvailable() && !await IsRunwayOccupied();
        }

        async Task<bool> IsRunwayOccupied()
        {
            return await dbContext.Airlines.AnyAsync(a => a.State == AirlineState.Landed || a.State == AirlineState.TakingOff);
        }

        async Task<bool> IsParkingLotAvailable()
        {
            return await dbContext.Airlines.CountAsync(a => a.State == AirlineState.Parked && a.Type == airline.Type) < AvailableParkingSlots;
        }

        int AvailableParkingSlots => airline.Type == AirlineType.Airliner ? airportSpecs.AirlineParkingSlots : airportSpecs.PrivateParkingSlots;

        async Task<bool> VerifyRunwayApproachability()
        {
            return await dbContext.Airlines.AnyAsync(a => a.State != AirlineState.Approach);
        }
        void StateChanged(object? sender, TransitionChangedEventArgs e)
        {
            dbContext.StateChangeHistory.Add(CreateHistory(HistoryStatus.Accepted));
            dbContext.SaveChanges();
            logger.LogInformation("Changed state of airline {callSign} from {state1} to {state2}", airline.CallSign, e.PreviouState, e.CurrentState);
        }

        StateChangeHistory CreateHistory(HistoryStatus status)
        {
            return new StateChangeHistory
            {
                AirlineId = airline.Id,
                Status = status
            };
        }
    }
}