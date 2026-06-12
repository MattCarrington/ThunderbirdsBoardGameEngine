using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Loader
{
    public class MapEdgeCatalogTests
    {
        [Fact]
        public void MapEdgeCatalogLoadsSuccessfully()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<IMapEdgeDefinitionCatalog>();

            // Act
            var result = catalog.GetAll();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(29, result.Length);
        }
    }
}
