using AirportControlTower.API.Models;
using Stateless;

namespace AirportControlTower.API.Application.FSM
{
    public class AirlineStateMachine
    {
        const string runwayAvailabilityGuardDescription = "runway available guard";
        const string runwayApproachabilityGuardDescription = "runway approachable guard";

        public event EventHandler<TransitionChangedEventArgs> StateChanged = default!;
        public event EventHandler<TransitionChangedFailedEventArgs> StateChangedFailed = default!;

        readonly StateMachine<AirlineState, AirlineStateTrigger> _stateMachine;
        public AirlineStateMachine(Airline airline,
            bool isRunwayAvailable, //is available also if there are parking slots, if can land even when there are no parking slots, its assumed runaway cannot be used, so why land on it in the first place
            bool isRunwayApproachable
            )
        {
            _stateMachine = new StateMachine<AirlineState, AirlineStateTrigger>(() => airline.State, s => airline.State = s);

            _stateMachine.Configure(AirlineState.Parked)
                .PermitIf(AirlineStateTrigger.TakeOff, AirlineState.TakingOff, () => isRunwayAvailable, runwayAvailabilityGuardDescription);

            _stateMachine.Configure(AirlineState.TakingOff) //only one on the runway at a time
                .Permit(AirlineStateTrigger.IsAirborne, AirlineState.Airborne);

            //check if no aircraft is on the approach
            //assumption: you can land if there isnt any parking space
            _stateMachine.Configure(AirlineState.Airborne)
                .PermitIf(AirlineStateTrigger.Approach, AirlineState.Approach, () => isRunwayApproachable, runwayApproachabilityGuardDescription);

            _stateMachine.Configure(AirlineState.Approach)
                .PermitIf(AirlineStateTrigger.Land, AirlineState.Landed, () => isRunwayAvailable, runwayAvailabilityGuardDescription)
                .Permit(AirlineStateTrigger.Abort, AirlineState.Airborne);

            _stateMachine.Configure(AirlineState.Landed) //only one on the runway at a time
                .Permit(AirlineStateTrigger.Park, AirlineState.Parked);


            _stateMachine.OnTransitionCompleted((sm) =>
            {
                OnStateChanged(new TransitionChangedEventArgs
                {
                    PreviouState = sm.Source,
                    CurrentState = sm.Destination
                });
            });


            _stateMachine.OnUnhandledTrigger((state, trigger) =>
           {
               OnStateChangedFailed(new TransitionChangedFailedEventArgs
               {
                   State = state
               });
           });
        }

        public virtual void Fire(AirlineStateTrigger trigger)
        {
            _stateMachine.Fire(trigger);
        }

        public bool CanFire(AirlineStateTrigger trigger)
        {
            return _stateMachine.CanFire(trigger);
        }

        protected virtual void OnStateChanged(TransitionChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        protected virtual void OnStateChangedFailed(TransitionChangedFailedEventArgs e)
        {
            StateChangedFailed?.Invoke(this, e);
        }
    }

    public class TransitionChangedEventArgs : EventArgs
    {
        public AirlineState PreviouState { get; set; }
        public AirlineState CurrentState { get; set; }
    }

    public class TransitionChangedFailedEventArgs : EventArgs
    {
        public AirlineState State { get; set; }
    }
}
