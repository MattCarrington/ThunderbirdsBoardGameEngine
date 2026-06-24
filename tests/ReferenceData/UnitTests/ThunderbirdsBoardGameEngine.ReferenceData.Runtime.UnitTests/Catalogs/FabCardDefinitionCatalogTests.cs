using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs
{
    public class FabCardDefinitionCatalogTests
    {
        [Fact]
        public void Exists_WhenExistingCardCode_ReturnsTrue()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.Exists(new CardCode("fab-1"));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_WhenNonExistingCardCode_ReturnsFalse()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.Exists(new CardCode("non-existing-code"));

            // Assert
            Assert.False(result);
        }

        private static FabCardDefinitionCatalog CreateCatalog()
        {
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithFabCard("fab-1", "Fab 1")
                .WithFabCard("fab-2", "Fab 2")
                .Build();

            return new FabCardDefinitionCatalog(snapshot);
        }
    }
}
