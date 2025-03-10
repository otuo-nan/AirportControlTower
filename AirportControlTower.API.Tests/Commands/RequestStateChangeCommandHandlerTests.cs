using AirportControlTower.API.Application.Commands;
using AirportControlTower.API.Infrastructure;
using AirportControlTower.API.Infrastructure.Configurations;
using AirportControlTower.API.Infrastructure.Database;
using AirportControlTower.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AirportControlTower.API.Tests.Commands
{
    public class RequestStateChangeCommandHandlerTests
    {
        private readonly Mock<ILogger<RequestChangeCommandHandler>> _loggerMock;
        private readonly Mock<IOptions<AirportSpecs>> _optionsMock;
        private readonly AppDbContext _dbContext;

        public RequestStateChangeCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<RequestChangeCommandHandler>>();
            _optionsMock = new Mock<IOptions<AirportSpecs>>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new AppDbContext(options);

            _optionsMock.Setup(x => x.Value).Returns(new AirportSpecs
            {
                AirlineParkingSlots = 5,
                PrivateParkingSlots = 3
            });
        }

        [Fact]
        public async Task Handle_WhenParkedAirlineTakesOff_ShouldSucceed()
        {
            // Arrange
            var airline = new Airline
            {
                Id = Guid.NewGuid(),
                Name = "Emirates",
                CallSign = "TEST123",
                State = AirlineState.Parked,
                Type = AirlineType.Airliner,
                LastKnownPosition = new Position()
            };
            _dbContext.Airlines.Add(airline);
            await _dbContext.SaveChangesAsync();

            var command = new RequestStateChangeCommand
            {
                CallSign = "TEST123",
                State = AirlineStateTrigger.TakeOff
            };

            var handler = new RequestChangeCommandHandler(_dbContext, _loggerMock.Object, _optionsMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var updatedAirline = await _dbContext.Airlines.FirstAsync(a => a.CallSign == "TEST123");
            Assert.Equal(AirlineState.TakingOff, updatedAirline.State);
        }

        [Fact]
        public async Task Handle_WhenRunwayOccupied_ShouldThrowPlatformException()
        {
            // Arrange
            var occupyingAirline = new Airline
            {
                Id = Guid.NewGuid(),
                Name = "Lufthansa",
                CallSign = "OCCUPY1",
                State = AirlineState.TakingOff,
                Type = AirlineType.Airliner,
                LastKnownPosition = new Position()
            };

            var requestingAirline = new Airline
            {
                Id = Guid.NewGuid(),
                Name = "Emirates",
                CallSign = "TEST123",
                State = AirlineState.Parked,
                Type = AirlineType.Airliner,
                LastKnownPosition = new Position()
            };

            _dbContext.Airlines.AddRange(occupyingAirline, requestingAirline);
            await _dbContext.SaveChangesAsync();

            var command = new RequestStateChangeCommand
            {
                CallSign = "TEST123",
                State = AirlineStateTrigger.TakeOff
            };

            var handler = new RequestChangeCommandHandler(_dbContext, _loggerMock.Object, _optionsMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<PlatformException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenParkingSlotsAreFull_ShouldThrowPlatformException()
        {
            // Arrange
            var parkedAirlines = Enumerable.Range(0, 5).Select(i => new Airline
            {
                Id = Guid.NewGuid(),
                Name = "Emirates",
                CallSign = $"PARKED{i}",
                State = AirlineState.Parked,
                Type = AirlineType.Airliner,
                LastKnownPosition = new Position()
            }).ToList();

            var approachingAirline = new Airline
            {
                Id = Guid.NewGuid(),
                Name = "Lufthansa",
                CallSign = "TEST123",
                State = AirlineState.Approach,
                Type = AirlineType.Airliner,
                LastKnownPosition = new Position()
            };

            _dbContext.Airlines.AddRange(parkedAirlines);
            _dbContext.Airlines.Add(approachingAirline);
            await _dbContext.SaveChangesAsync();

            var command = new RequestStateChangeCommand
            {
                CallSign = "TEST123",
                State = AirlineStateTrigger.Land
            };

            var handler = new RequestChangeCommandHandler(_dbContext, _loggerMock.Object, _optionsMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<PlatformException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenAnotherAircraftIsApproaching_ShouldThrowPlatformException()
        {
            // Arrange
            var approachingAirline = new Airline
            {
                Id = Guid.NewGuid(),
                Name = "Emirates",
                CallSign = "APPR123",
                State = AirlineState.Approach,
                Type = AirlineType.Airliner,
                LastKnownPosition = new Position()
            };

            var requestingAirline = new Airline
            {
                Id = Guid.NewGuid(),
                Name = "Lufthansa",
                CallSign = "TEST456",
                State = AirlineState.Airborne,
                Type = AirlineType.Airliner,
                LastKnownPosition = new Position()
            };

            _dbContext.Airlines.AddRange(approachingAirline, requestingAirline);
            await _dbContext.SaveChangesAsync();

            var command = new RequestStateChangeCommand
            {
                CallSign = "TEST456",
                State = AirlineStateTrigger.Approach
            };

            var airlines = await _dbContext.Airlines.AsNoTracking().ToListAsync();

            var handler = new RequestChangeCommandHandler(_dbContext, _loggerMock.Object, _optionsMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PlatformException>(() =>
                handler.Handle(command, CancellationToken.None));

            // Verify state remains unchanged
            var airlineAfterRequest = await _dbContext.Airlines.FirstAsync(a => a.CallSign == "TEST456");
            Assert.Equal(AirlineState.Airborne, airlineAfterRequest.State);

            // Verify history record was created
            var historyRecord = await _dbContext.StateChangeHistory
                .OrderByDescending(h => h.CreatedOn)
                .FirstAsync();
            Assert.Equal(HistoryStatus.Rejected, historyRecord.Status);
            Assert.Equal(requestingAirline.Id, historyRecord.AirlineId);
        }
    }
}

