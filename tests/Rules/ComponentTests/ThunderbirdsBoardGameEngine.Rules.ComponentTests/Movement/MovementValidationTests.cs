using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Movement
{
    public class MovementValidationTests
    {
        [Fact]
        public async Task MovementShouldBeValidForEarthboundVehicle()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                Thunderbird: new ThunderbirdCode("thunderbird-1"),
                Start: new LocationCode("Europe"),
                Destination: new LocationCode("North America")
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.SpacesTravelled);
        }

        [Fact]
        public async Task MovementShouldBeValidForSpaceVehicle()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                Thunderbird: new ThunderbirdCode("thunderbird-3"),
                Start: new LocationCode("Moon"),
                Destination: new LocationCode("Pacific")
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.SpacesTravelled);
        }

        [Fact]
        public async Task MovementShouldBeInvalidDueToInvalidDestination()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                Thunderbird: new ThunderbirdCode("thunderbird-1"),
                Start: new LocationCode("Europe"),
                Destination: new LocationCode("North Pole")
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidMovementCalculationRequestException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task MovementShouldBeInvalidDueToNonExistentStartLocation()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                Thunderbird: new ThunderbirdCode("thunderbird-1"),
                Start: new LocationCode("Atlantis"),
                Destination: new LocationCode("North America")
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidMovementCalculationRequestException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task MovementShouldBeInvalidDueToNonExistentThunderbird()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                Thunderbird: new ThunderbirdCode("thunderbird-x"),
                Start: new LocationCode("Europe"),
                Destination: new LocationCode("North America")
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<ReferenceDataNotFoundException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task MovementShouldBeInvalidAsEarthboundVehicleCannotTravelToSpace()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                Thunderbird: new ThunderbirdCode("thunderbird-1"),
                Start: new LocationCode("Europe"),
                Destination: new LocationCode("Space")
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidMovementCalculationRequestException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task MovementShouldBeInvalidAsSpaceVehicleCannotTraverseEarthEdges()
        {
            // Arrange
            var request = new ValidateMovementQuery
            (
                Thunderbird: new ThunderbirdCode("thunderbird-3"),
                Start: new LocationCode("Space"),
                Destination: new LocationCode("Europe")
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidMovementCalculationRequestException>(() => mediator.Send(request, CancellationToken.None));
        }

        private static IMediator CreateMediator()
        {
            var edges = CreateEdges();
            var locations = CreateLocations();
            var thunderbirds = CreateThunderbirds();

            var services = new ServiceCollection();
            services.AddSingleton<IMapEdgeDefinitionCatalog>(edges);
            services.AddSingleton<ILocationDefinitionCatalog>(locations);
            services.AddSingleton<IThunderbirdDefinitionCatalog>(thunderbirds);
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
            return new FakeThunderbirdDefinitionCatalog(thunderbird1, thunderbird2, thunderbird3, thunderbird4);
        }
    }
}
