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
        int AvailableParkingSlots => airline.Type == AirlineType.Airliner ? airportSpecs.AirlineParkingSlots : airportSpecs.PrivateParkingSlots;

        public async Task Handle(RequestStateChangeCommand command, CancellationToken cancellationToken)
        {
            airline = await GetAirlineAsync(command.CallSign);

            var sm = new AirlineStateMachine(airline, await IsRunwayAvailable(), await IsRunwayApproachable());
            sm.StateChanged += StateChanged;
            sm.StateChangedFailed += StateChangedFailed;

            sm.Fire(command.State);

            void StateChanged(object? sender, TransitionChangedEventArgs e)
            {
                dbContext.StateChangeHistory.Add(CreateHistory(e.PreviouState, command.State, HistoryStatus.Accepted));
                dbContext.SaveChanges();
                logger.LogInformation("Changed state of airline {callSign} from state: {state1} with trigger: {trigger}", airline.CallSign, e.PreviouState, e.CurrentState);
            }

            void StateChangedFailed(object? sender, TransitionChangedFailedEventArgs e)
            {
                dbContext.StateChangeHistory.Add(CreateHistory(airline.State, command.State, HistoryStatus.Rejected));
                dbContext.SaveChanges();
                logger.LogError("Failed to change state of airline {callSign} from {state1} to {state2}", airline.CallSign, airline.State, e.State);
                throw new PlatformException { CustomStatusCode = (int)HttpStatusCode.Conflict };
            }
        }

        async Task<Airline> GetAirlineAsync(string callSign)
        {
            return await dbContext.Airlines.FirstAsync(a => a.CallSign == callSign);
        }

        async Task<bool> IsRunwayAvailable()
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

        async Task<bool> IsRunwayApproachable()
        {
            return await dbContext.Airlines.CountAsync(a => a.State == AirlineState.Approach) == 0 && !await IsRunwayOccupied();
        }

        StateChangeHistory CreateHistory(AirlineState fromState, AirlineStateTrigger trigger, HistoryStatus status)
        {
            return new StateChangeHistory
            {
                AirlineId = airline.Id,
                FromState = fromState,
                Trigger = trigger,
                Status = status
            };
        }
    }
}