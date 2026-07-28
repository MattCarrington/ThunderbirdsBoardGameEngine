using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Fixtures;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.TestData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Movement
{
    public class MovementValidationTests
    {
        [Theory]
        [InlineData("thunderbird-1", 1)]
        [InlineData("thunderbird-2", 1)]
        [InlineData("thunderbird-4", 2)]
        public async Task MovementShouldBeValidForEarthboundVehicle(string thunderbird, int expectedActionPointCost)
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode(thunderbird),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAmerica,
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(expectedActionPointCost, result.ActionPointCost);
        }

        [Fact]
        public async Task MovementShouldBeValidForSpaceVehicle()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-3"),
                StartLocationCode: TestLocationCodes.TheMoon,
                DestinationLocationCode: TestLocationCodes.SouthPacific,
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(1, result.ActionPointCost);
        }

        [Fact]
        public async Task MovementShouldBeInvalidDueToInvalidDestination()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-1"),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: new LocationCode("North Pole"),
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<ReferenceDataNotFoundException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task MovementShouldBeInvalidDueToNonExistentStartLocation()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-1"),
                StartLocationCode: new LocationCode("Atlantis"),
                DestinationLocationCode: TestLocationCodes.NorthAmerica,
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<ReferenceDataNotFoundException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task MovementShouldBeInvalidDueToNonExistentThunderbird()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-x"),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAmerica,
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<ReferenceDataNotFoundException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Theory]
        [InlineData("thunderbird-1")]
        [InlineData("thunderbird-2")]
        [InlineData("thunderbird-4")]
        public async Task MovementShouldBeInvalidAsEarthboundVehicleCannotTravelToSpace(string thunderbird)
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode(thunderbird),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.GeoStationaryOrbit,
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task MovementShouldBeInvalidAsSpaceVehicleCannotTraverseEarthEdges()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-3"),
                StartLocationCode: TestLocationCodes.GeoStationaryOrbit,
                DestinationLocationCode: TestLocationCodes.Europe,
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task MovementShouldBeInvalidAsVehicleCannotMove()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-5"),
                StartLocationCode: TestLocationCodes.TheMoon,
                DestinationLocationCode: TestLocationCodes.TheSun,
                ActiveEventCardCodes: Array.Empty<CardCode>()
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task MovementShouldBeRestrictedDueToActiveEventCard()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-3"),
                StartLocationCode: TestLocationCodes.SouthPacific,
                DestinationLocationCode: TestLocationCodes.TheMoon,
                ActiveEventCardCodes: [KnownEventCardCodes.RocketMalfunction]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(2, result.ActionPointCost);
            Assert.Equal(1, result.EffectiveTopSpeed);
            Assert.Equal(3, result.ThunderbirdTopSpeed);

            var message = Assert.Single(result.Messages);
            Assert.Equal("Rocket Malfunction: Thunderbird 3's top speed is reduced to 1.", message);
        }

        [Fact]
        public async Task MovementShouldNotBeRestrictedDueToEventCardNotApplyingToThunderbird()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-2"),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAmerica,
                ActiveEventCardCodes: [KnownEventCardCodes.AttackOfTheZombites]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(1, result.ActionPointCost);
            Assert.Equal(2, result.EffectiveTopSpeed);
            Assert.Equal(2, result.ThunderbirdTopSpeed);
            Assert.Empty(result.Messages);
        }

        [Fact]
        public async Task MovementShouldNotBeRestrictedAsEventCardDoesNotAffectMovement()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-1"),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAmerica,
                ActiveEventCardCodes: [new CardCode("explosion-on-tracy-island")]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(1, result.ActionPointCost);
            Assert.Equal(3, result.ThunderbirdTopSpeed);
            Assert.Equal(3, result.EffectiveTopSpeed);
            Assert.Empty(result.Messages);
        }

        [Fact]
        public async Task MovementShouldTakeAlternativeRouteWhenIcelandicVolcanoEruptionBlocksDirectEdge()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: KnownThunderbirdCodes.Thunderbird2,
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAtlantic,
                ActiveEventCardCodes: [KnownEventCardCodes.IcelandicVolcanoEruption]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(4, result.SpacesTravelled);
            Assert.Equal(
                ["europe", "asia", "south-pacific", "north-america", "north-atlantic"],
                result.Route.Select(location => location.Value));
            Assert.Contains(result.Messages, message => message.Contains("Icelandic Volcano Eruption"));
        }

        [Fact]
        public async Task MovementShouldApplyTopologyAndSpeedModifiersFromDifferentEventCards()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: KnownThunderbirdCodes.Thunderbird2,
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAtlantic,
                ActiveEventCardCodes:
                [
                    KnownEventCardCodes.IcelandicVolcanoEruption,
                    KnownEventCardCodes.UsnSentinelMissileStrike
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(4, result.SpacesTravelled);
            Assert.Equal(2, result.ThunderbirdTopSpeed);
            Assert.Equal(1, result.EffectiveTopSpeed);
            Assert.Equal(4, result.ActionPointCost);
            Assert.Equal(
                ["europe", "asia", "south-pacific", "north-america", "north-atlantic"],
                result.Route.Select(location => location.Value));
            Assert.Contains(result.Messages, message => message.Contains("Icelandic Volcano Eruption"));
            Assert.Contains(result.Messages, message => message.Contains("USN Sentinel Missile Strike"));
        }

        [Theory]
        [InlineData("thunderbird-1", "Attack of the Zombites: Thunderbird 1's top speed is reduced to 1.", 3)]
        [InlineData("thunderbird-2", "USN Sentinel Missile Strike: Thunderbird 2's top speed is reduced to 1.", 2)]
        public async Task MovementCanOnlyBeAffectedByOneEventCard(string thunderbirdCode, string expectedMessage, int baseSpeed)
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode(thunderbirdCode),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAmerica,
                ActiveEventCardCodes: [KnownEventCardCodes.AttackOfTheZombites, KnownEventCardCodes.UsnSentinelMissileStrike, KnownEventCardCodes.RocketMalfunction]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(2, result.ActionPointCost);
            Assert.Equal(1, result.EffectiveTopSpeed);
            Assert.Equal(baseSpeed, result.ThunderbirdTopSpeed);

            var message = Assert.Single(result.Messages);
            Assert.Equal(expectedMessage, message);
        }

        [Fact]
        public async Task ThrowsInvalidReferenceDataNotFoundExceptionWhenEventCardDoesNotExist()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode("thunderbird-1"),
                StartLocationCode: TestLocationCodes.Europe,
                DestinationLocationCode: TestLocationCodes.NorthAmerica,
                ActiveEventCardCodes: [new CardCode("non-existent-event-card")]
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<ReferenceDataNotFoundException>(() => mediator.Send(request, CancellationToken.None));
        }

        private static IMediator CreateMediator()
        {
            var services = new ServiceCollection();

            services.AddRules();
            services.AddFakeCatalogs();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }
    }
}
