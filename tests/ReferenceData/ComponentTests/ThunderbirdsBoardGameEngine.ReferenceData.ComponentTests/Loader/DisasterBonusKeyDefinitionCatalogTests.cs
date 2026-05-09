using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Loader
{
    public class DisasterBonusKeyDefinitionCatalogTests
    {
        [Fact]
        public void CanLoadCharacterDisasterBonusKeyDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();
            var catalog = provider.GetRequiredService<IDisasterBonusKeyDefintionCatalog>();

            // Act
            var result = catalog.GetByCode(new DisasterBonusKey("gordon"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("gordon", result.Key.ToString());
            Assert.Equal("Gordon", result.DisplayName);
        }

        [Fact]
        public void CanLoadThunderbirdDisasterBonusKeyDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();
            var catalog = provider.GetRequiredService<IDisasterBonusKeyDefintionCatalog>();

            // Act
            var result = catalog.GetByCode(new DisasterBonusKey("thunderbird-1"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("thunderbird-1", result.Key.ToString());
            Assert.Equal("Thunderbird 1", result.DisplayName);
        }

        [Fact]
        public void CanLoadPodVehicleDisasterBonusKeyDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();
            var catalog = provider.GetRequiredService<IDisasterBonusKeyDefintionCatalog>();

            // Act
            var result = catalog.GetByCode(new DisasterBonusKey("domo"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("domo", result.Key.ToString());
            Assert.Equal("DOMO", result.DisplayName);
        }
    }
}