using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AirportControlTower.API.Application.Commands
{
    public class ShareLastKnownLocationCommand : IRequest
    {
        public required string CallSign { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public float Heading { get; set; }
    }

    public class LastKnownLocationCommandHandler(AppDbContext dbContext,
        ILogger<LastKnownLocationCommandHandler> logger) : IRequestHandler<ShareLastKnownLocationCommand>
    {
        public async Task Handle(ShareLastKnownLocationCommand request, CancellationToken cancellationToken)
        {
            await dbContext.Airlines.Where(a => a.CallSign == request.CallSign)
                 .ExecuteUpdateAsync(setter => setter.SetProperty(p => p.LastKnownPosition,
                 new Position
                 {
                     Latitude = request.Latitude,
                     Longitude = request.Longitude,
                     Altitude = request.Altitude,
                     Heading = request.Heading
                 })
                 .SetProperty(p => p.LastUpdate, DateTime.UtcNow)
           , cancellationToken: cancellationToken);
        }
    }
}
