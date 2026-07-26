using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fixtures;
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
                TestLocationCodes.SouthPacific
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
                TestLocationCodes.GeoStationaryOrbit,
                TestLocationCodes.TheMoon,
                TestLocationCodes.TheSun,
                TestLocationCodes.SouthPacific
            };

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessibleLocations);
            Assert.All(expectedLocations, location => Assert.Contains(location, result.AccessibleLocations));
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
