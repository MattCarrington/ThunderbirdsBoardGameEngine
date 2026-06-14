using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Movement
{
    public class AccessibleLocationsTests
    {
        [Theory]
        [InlineData("thunderbird-1")]
        [InlineData("thunderbird-2")]
        [InlineData("thunderbird-4")]
        public async Task EarthboundAccessibleLocations(string thunderbirdCode)
        {
            // Arrange
            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(new FindAccessibleLocationsQuery(new ThunderbirdCode(thunderbirdCode)), TestContext.Current.CancellationToken);

            // Assert
            var expectedLocations = new[]
            {
                new LocationCode("Europe"),
                new LocationCode("Asia"),
                new LocationCode("Africa"),
                new LocationCode("Australia"),
                new LocationCode("North America"),
                new LocationCode("South America"),
                new LocationCode("Atlantic"),
                new LocationCode("Pacific")
            };

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessibleLocations);
            Assert.Equal(expectedLocations.Length, result.AccessibleLocations.Count);
            Assert.All(expectedLocations, location => Assert.Contains(location, result.AccessibleLocations));
        }

        [Theory]
        [InlineData("thunderbird-3")]
        [InlineData("thunderbird-5")]
        public async Task SpaceAccessibleLocations(string thunderbirdCode)
        {
            // Arrange
            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(new FindAccessibleLocationsQuery(new ThunderbirdCode(thunderbirdCode)), TestContext.Current.CancellationToken);

            // Assert
            var expectedLocations = new[]
            {
                new LocationCode("Space"),
                new LocationCode("Moon"),
                new LocationCode("Sun"),
                new LocationCode("Pacific")
            };

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessibleLocations);
            Assert.All(expectedLocations, location => Assert.Contains(location, result.AccessibleLocations));
        }

        private static IMediator CreateMediator()
        {
            var edges = CreateEdges();
            var thunderbirds = CreateThunderbirds();

            var services = new ServiceCollection();
            services.AddSingleton<IMapEdgeDefinitionCatalog>(edges);
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

        private static FakeThunderbirdDefinitionCatalog CreateThunderbirds()
        {
            var thunderbird1 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-1"), "Thunderbird 1", MovementDomain.Earth, 3);
            var thunderbird2 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-2"), "Thunderbird 2", MovementDomain.Earth, 2);
            var thunderbird3 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-3"), "Thunderbird 3", MovementDomain.Space, 3);
            var thunderbird4 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-4"), "Thunderbird 4", MovementDomain.Earth, 1);
            var thunderbird5 = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-5"), "Thunderbird 5", MovementDomain.Space, 0);

            return new FakeThunderbirdDefinitionCatalog(thunderbird1, thunderbird2, thunderbird3, thunderbird4, thunderbird5);
        }
    }
}
