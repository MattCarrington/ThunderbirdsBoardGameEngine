using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs
{
    public class MapEdgeDefinitionCatalogTests
    {
        [Fact]
        public void GetAll_WithValidSnapshot_ReturnsAllMapEdges()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetAll();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Contains(result, e => e.Edge1 == new LocationCode("edge-1") && e.Edge2 == new LocationCode("edge-2") && e.EdgeType == MovementDomain.Earth);
            Assert.Contains(result, e => e.Edge1 == new LocationCode("edge-2") && e.Edge2 == new LocationCode("edge-3") && e.EdgeType == MovementDomain.Space);
        }

        [Fact]
        public void Constructor_WithNullSnapshot_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MapEdgeDefinitionCatalog(null!));
        }

        private static MapEdgeDefinitionCatalog CreateCatalog()
        {
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithMapEdge("edge-1", "edge-2", MovementDomain.Earth)
                .WithMapEdge("edge-2", "edge-3", MovementDomain.Space)
                .Build();

            return new MapEdgeDefinitionCatalog(snapshot);
        }
    }
}
