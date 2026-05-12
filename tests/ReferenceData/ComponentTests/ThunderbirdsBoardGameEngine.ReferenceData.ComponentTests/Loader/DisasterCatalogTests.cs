using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Loader
{
    public class DisasterCatalogTests
    {
        [Fact]
        public void CanLoadDisasterDefinitions()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<IDisasterDefinitionCatalog>();

            // Act            
            var result = catalog.GetAll();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(34, result.Length);
        }

        [Fact]
        public void CanLoadKnownDisasterDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<IDisasterDefinitionCatalog>();

            // Act            
            var result = catalog.GetByCode(new CardCode("terror-in-new-york-city"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("terror-in-new-york-city", result.Code.ToString());
            Assert.Equal("Terror in New York City", result.DisplayName);
        }
    }
}
