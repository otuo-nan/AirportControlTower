using AirportControlTower.API.Models;

namespace AirportControlTower.API.Application.Requests
{
    public class ControlTowerRequests
    {
        public class RequestStateChange
        {
            public AirlineStateTrigger State { get; set; }
        }
    }
}
