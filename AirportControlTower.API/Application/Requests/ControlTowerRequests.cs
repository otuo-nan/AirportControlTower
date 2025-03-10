using AirportControlTower.API.Models;

namespace AirportControlTower.API.Application.Requests
{
    public class ControlTowerRequests
    {
        public class ChangeState
        {
            public AirlineStateTrigger State { get; set; }
        }

        public class SharedLocation : Position { }
    }
}
