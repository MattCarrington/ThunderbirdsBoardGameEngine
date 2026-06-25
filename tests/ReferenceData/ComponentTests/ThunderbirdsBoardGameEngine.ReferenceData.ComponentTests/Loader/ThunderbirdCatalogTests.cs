using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Loader
{
    public class ThunderbirdCatalogTests
    {
        [Fact]
        public void CanLoadAllThunderbirdDefinitions()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<IThunderbirdDefinitionCatalog>();

            // Act            
            var result = catalog.GetAll();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(6, result.Length);
        }

        [Fact]
        public void CanLoadKnownThunderbirdDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<IThunderbirdDefinitionCatalog>();

            // Act            
            var result = catalog.TryGetByCode(new ThunderbirdCode("thunderbird-1"), out var definition);

            // Assert
            Assert.True(result);
            Assert.NotNull(definition);
            Assert.Equal("thunderbird-1", definition.Code.ToString());
            Assert.Equal("Thunderbird 1", definition.DisplayName);
            Assert.Equal(MovementDomain.Earth, definition.Domain);
            Assert.Equal(3, definition.TopSpeed);
        }
    }
}
