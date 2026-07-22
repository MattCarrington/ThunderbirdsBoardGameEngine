using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
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
                StartLocationCode: new LocationCode("Europe"),
                DestinationLocationCode: new LocationCode("North America"),
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
                StartLocationCode: new LocationCode("Moon"),
                DestinationLocationCode: new LocationCode("Pacific"),
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
                StartLocationCode: new LocationCode("Europe"),
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
                DestinationLocationCode: new LocationCode("North America"),
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
                StartLocationCode: new LocationCode("Europe"),
                DestinationLocationCode: new LocationCode("North America"),
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
                StartLocationCode: new LocationCode("Europe"),
                DestinationLocationCode: new LocationCode("Space"),
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
                StartLocationCode: new LocationCode("Space"),
                DestinationLocationCode: new LocationCode("Europe"),
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
                StartLocationCode: new LocationCode("Moon"),
                DestinationLocationCode: new LocationCode("Sun"),
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
                StartLocationCode: new LocationCode("Pacific"),
                DestinationLocationCode: new LocationCode("Moon"),
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
                StartLocationCode: new LocationCode("Europe"),
                DestinationLocationCode: new LocationCode("North America"),
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
                StartLocationCode: new LocationCode("Europe"),
                DestinationLocationCode: new LocationCode("North America"),
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

        [Theory]
        [InlineData("thunderbird-1", "Attack of the Zombites: Thunderbird 1's top speed is reduced to 1.", 3)]
        [InlineData("thunderbird-2", "USN Sentinel Missile Strike: Thunderbird 2's top speed is reduced to 1.", 2)]
        public async Task MovementCanOnlyBeAffectedByOneEventCard(string thunderbirdCode, string expectedMessage, int baseSpeed)
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode(thunderbirdCode),
                StartLocationCode: new LocationCode("Europe"),
                DestinationLocationCode: new LocationCode("North America"),
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
                StartLocationCode: new LocationCode("Europe"),
                DestinationLocationCode: new LocationCode("North America"),
                ActiveEventCardCodes: [new CardCode("non-existent-event-card")]
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<ReferenceDataNotFoundException>(() => mediator.Send(request, CancellationToken.None));
        }

        private static IMediator CreateMediator()
        {
            var edges = CreateEdges();
            var locations = CreateLocations();
            var thunderbirds = CreateThunderbirds();
            var eventCards = CreateEventCards();

            var services = new ServiceCollection();
            services.AddSingleton<IMapEdgeDefinitionCatalog>(edges);
            services.AddSingleton<ILocationDefinitionCatalog>(locations);
            services.AddSingleton<IThunderbirdDefinitionCatalog>(thunderbirds);
            services.AddSingleton<IEventCardDefinitionCatalog>(eventCards);
            services.AddRules();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }

        private static FakeMapEdgeDefinitionCatalog CreateEdges()
        {
            var europeToAsia = new ReferenceMapEdgeDefinition(new LocationCode("Europe"), new LocationCode("Asia"), MovementDomain.Earth);
            var europeToAfrica = new ReferenceMapEdgeDefinition(new LocationCode("Europe"), new LocationCode("Africa"), MovementDomain.Earth);
            var asiaToAfrica = new ReferenceMapEdgeDefinition(new LocationCode("Asia"), new LocationCode("Africa"), MovementDomain.Earth);
            var asiaToAustralia = new ReferenceMapEdgeDefinition(new LocationCode("Asia"), new LocationCode("Australia"), MovementDomain.Earth);
            var africaToAustralia = new ReferenceMapEdgeDefinition(new LocationCode("Africa"), new LocationCode("Australia"), MovementDomain.Earth);
            var northAmericaToSouthAmerica = new ReferenceMapEdgeDefinition(new LocationCode("North America"), new LocationCode("South America"), MovementDomain.Earth);
            var atlanticToEurope = new ReferenceMapEdgeDefinition(new LocationCode("Atlantic"), new LocationCode("Europe"), MovementDomain.Earth);
            var atlanticToAfrica = new ReferenceMapEdgeDefinition(new LocationCode("Atlantic"), new LocationCode("Africa"), MovementDomain.Earth);
            var atlanticToNorthAmerica = new ReferenceMapEdgeDefinition(new LocationCode("Atlantic"), new LocationCode("North America"), MovementDomain.Earth);
            var atlanticToSouthAmerica = new ReferenceMapEdgeDefinition(new LocationCode("Atlantic"), new LocationCode("South America"), MovementDomain.Earth);
            var pacificToAustralia = new ReferenceMapEdgeDefinition(new LocationCode("Pacific"), new LocationCode("Australia"), MovementDomain.Earth);
            var pacificToAsia = new ReferenceMapEdgeDefinition(new LocationCode("Asia"), new LocationCode("Pacific"), MovementDomain.Earth);
            var pacificToNorthAmerica = new ReferenceMapEdgeDefinition(new LocationCode("Pacific"), new LocationCode("North America"), MovementDomain.Earth);
            var pacificToSouthAmerica = new ReferenceMapEdgeDefinition(new LocationCode("Pacific"), new LocationCode("South America"), MovementDomain.Earth);
            var pacificToSpace = new ReferenceMapEdgeDefinition(new LocationCode("Pacific"), new LocationCode("Space"), MovementDomain.Space);
            var spaceToMoon = new ReferenceMapEdgeDefinition(new LocationCode("Space"), new LocationCode("Moon"), MovementDomain.Space);
            var spaceToSun = new ReferenceMapEdgeDefinition(new LocationCode("Space"), new LocationCode("Sun"), MovementDomain.Space);

            return new FakeMapEdgeDefinitionCatalog(
                europeToAsia,
                europeToAfrica,
                asiaToAfrica,
                asiaToAustralia,
                africaToAustralia,
                northAmericaToSouthAmerica,
                pacificToAsia,
                asiaToAustralia,
                northAmericaToSouthAmerica,
                atlanticToEurope,
                atlanticToAfrica,
                atlanticToNorthAmerica,
                atlanticToSouthAmerica,
                pacificToAustralia,
                pacificToNorthAmerica,
                pacificToSouthAmerica,
                pacificToSpace,
                spaceToMoon,
                spaceToSun);
        }

        private static FakeLocationDefinitionCatalog CreateLocations()
        {
            var europe = new ReferenceLocationDefinition(new LocationCode("Europe"), "Europe", MovementDomain.Earth);
            var asia = new ReferenceLocationDefinition(new LocationCode("Asia"), "Asia", MovementDomain.Earth);
            var northAmerica = new ReferenceLocationDefinition(new LocationCode("North America"), "North America", MovementDomain.Earth);
            var southAmerica = new ReferenceLocationDefinition(new LocationCode("South America"), "South America", MovementDomain.Earth);
            var atlantic = new ReferenceLocationDefinition(new LocationCode("Atlantic"), "Atlantic Ocean", MovementDomain.Earth);
            var pacific = new ReferenceLocationDefinition(new LocationCode("Pacific"), "Pacific Ocean", MovementDomain.Earth);
            var australia = new ReferenceLocationDefinition(new LocationCode("Australia"), "Australia", MovementDomain.Earth);
            var africa = new ReferenceLocationDefinition(new LocationCode("Africa"), "Africa", MovementDomain.Earth);
            var space = new ReferenceLocationDefinition(new LocationCode("Space"), "Space", MovementDomain.Space);
            var moon = new ReferenceLocationDefinition(new LocationCode("Moon"), "Moon", MovementDomain.Space);
            var sun = new ReferenceLocationDefinition(new LocationCode("Sun"), "Sun", MovementDomain.Space);

            return new FakeLocationDefinitionCatalog(europe, asia, northAmerica, southAmerica, atlantic, pacific, australia, africa, space, moon, sun);
        }

        private static FakeThunderbirdDefinitionCatalog CreateThunderbirds()
        {
            var thunderbird1 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-1"), "Thunderbird 1", MovementDomain.Earth, 3);
            var thunderbird2 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-2"), "Thunderbird 2", MovementDomain.Earth, 2);
            var thunderbird3 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-3"), "Thunderbird 3", MovementDomain.Space, 3);
            var thunderbird4 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-4"), "Thunderbird 4", MovementDomain.Earth, 1);
            var thunderbird5 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-5"), "Thunderbird 5", MovementDomain.Space, 0);

            return new FakeThunderbirdDefinitionCatalog(thunderbird1, thunderbird2, thunderbird3, thunderbird4, thunderbird5);
        }

        private static FakeEventCardDefinitionCatalog CreateEventCards()
        {
            var attackOfTheZombites = new ReferenceEventCardDefinition(KnownEventCardCodes.AttackOfTheZombites, "Attack of the Zombites");
            var usnSentinelMissileStrike = new ReferenceEventCardDefinition(KnownEventCardCodes.UsnSentinelMissileStrike, "USN Sentinel Missile Strike");
            var rocketMalfunction = new ReferenceEventCardDefinition(KnownEventCardCodes.RocketMalfunction, "Rocket Malfunction");
            var explosionOnTracyIsland = new ReferenceEventCardDefinition(new CardCode("explosion-on-tracy-island"), "Explosion on Tracy Island");

            return new FakeEventCardDefinitionCatalog(attackOfTheZombites, usnSentinelMissileStrike, rocketMalfunction, explosionOnTracyIsland);
        }
    }
}
