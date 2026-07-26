using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fixture;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData;
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
                TestLocationCodes.Europe,
                TestLocationCodes.Asia,
                TestLocationCodes.Africa,
                TestLocationCodes.Australia,
                TestLocationCodes.NorthAmerica,
                TestLocationCodes.SouthAmerica,
                TestLocationCodes.NorthAtlantic,
                TestLocationCodes.Pacific
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
                TestLocationCodes.Space,
                TestLocationCodes.Moon,
                TestLocationCodes.Sun,
                TestLocationCodes.Pacific
            };

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessibleLocations);
            Assert.All(expectedLocations, location => Assert.Contains(location, result.AccessibleLocations));
        }

        private static IMediator CreateMediator()
        {
            var thunderbirds = CreateThunderbirds();

            var services = new ServiceCollection();
            services.AddSingleton<IThunderbirdDefinitionCatalog>(thunderbirds);
            services.AddRules();
            services.AddFakeCatalogs();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
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
