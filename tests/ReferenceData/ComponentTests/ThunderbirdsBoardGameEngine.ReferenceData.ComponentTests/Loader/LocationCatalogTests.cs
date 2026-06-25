using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Loader
{
    public class LocationCatalogTests
    {
        [Fact]
        public void CanLoadLocationDefinitions()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<ILocationDefinitionCatalog>();

            // Act            
            var result = catalog.GetAll();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(18, result.Length);
        }

        [Fact]
        public void CanLoadKnownLocationDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<ILocationDefinitionCatalog>();

            // Act            
            var result = catalog.TryGetByCode(new LocationCode("north-america"), out var location);

            // Assert
            Assert.True(result);
            Assert.NotNull(location);
            Assert.Equal("north-america", location.Code.Value);
            Assert.Equal("North America", location.DisplayName);
            Assert.Equal(MovementDomain.Earth, location.Domain);
        }
    }
}
