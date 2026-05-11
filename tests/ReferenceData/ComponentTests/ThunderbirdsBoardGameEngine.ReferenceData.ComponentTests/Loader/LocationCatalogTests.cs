using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
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
            var result = catalog.GetByCode(new LocationCode("north-america"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("north-america", result.Code.ToString());
            Assert.Equal("North America", result.DisplayName);
        }
    }
}
