using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
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
        public async Task MovementShouldBeValid()
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
            var result = await mediator.Send(request);

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
                Destination: new LocationCode("Space")
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidMovementCalculationRequestException>(() => mediator.Send(request));
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
            await Assert.ThrowsAsync<InvalidMovementCalculationRequestException>(() => mediator.Send(request));
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
            var asiaToNorthAmerica = new ReferenceMapEdgeDefinition(new LocationCode("Asia"), new LocationCode("North America"), MovementDomain.Earth);
            var northAmericaToNorthAtlantic = new ReferenceMapEdgeDefinition(new LocationCode("North America"), new LocationCode("North Atlantic"), MovementDomain.Earth);
            var asiaToPacific = new ReferenceMapEdgeDefinition(new LocationCode("Asia"), new LocationCode("Pacific"), MovementDomain.Earth);
            var northAtlanticToEurope = new ReferenceMapEdgeDefinition(new LocationCode("North Atlantic"), new LocationCode("Europe"), MovementDomain.Earth);
            var pacificToSpace = new ReferenceMapEdgeDefinition(new LocationCode("Pacific"), new LocationCode("Space"), MovementDomain.Space);

            return new FakeMapEdgeDefinitionCatalog(europeToAsia, asiaToNorthAmerica, northAmericaToNorthAtlantic, asiaToPacific, northAtlanticToEurope, pacificToSpace);
        }

        private static FakeLocationDefinitionCatalog CreateLocations()
        {
            var europe = new ReferenceLocationDefinition(new LocationCode("Europe"), "Europe", MovementDomain.Earth);
            var asia = new ReferenceLocationDefinition(new LocationCode("Asia"), "Asia", MovementDomain.Earth);
            var northAmerica = new ReferenceLocationDefinition(new LocationCode("North America"), "North America", MovementDomain.Earth);
            var northAtlantic = new ReferenceLocationDefinition(new LocationCode("North Atlantic"), "North Atlantic", MovementDomain.Earth);
            var pacific = new ReferenceLocationDefinition(new LocationCode("Pacific"), "Pacific", MovementDomain.Earth);
            var space = new ReferenceLocationDefinition(new LocationCode("Space"), "Space", MovementDomain.Space);

            return new FakeLocationDefinitionCatalog(europe, asia, northAmerica, northAtlantic, pacific, space);
        }

        private static FakeThunderbirdDefinitionCatalog CreateThunderbirds()
        {
            var thunderbird1 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-1"), "Thunderbird 1", MovementDomain.Earth);
            var thunderbird2 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-2"), "Thunderbird 2", MovementDomain.Earth);
            var thunderbird3 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-3"), "Thunderbird 3", MovementDomain.Space);
            var thunderbird4 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-4"), "Thunderbird 4", MovementDomain.Earth);
            return new FakeThunderbirdDefinitionCatalog(thunderbird1, thunderbird2, thunderbird3, thunderbird4);
        }
    }
}
