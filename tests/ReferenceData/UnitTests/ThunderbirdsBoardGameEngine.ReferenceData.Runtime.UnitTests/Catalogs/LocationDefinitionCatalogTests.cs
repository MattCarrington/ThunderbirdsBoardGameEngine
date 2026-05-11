using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs
{
    public class LocationDefinitionCatalogTests
    {
        [Fact]
        public void GetAll_WithValidSnapshot_ReturnsAllLocations()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetAll();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Contains(result, d => d.Code == new LocationCode("location-1") && d.DisplayName == "Location 1");
            Assert.Contains(result, d => d.Code == new LocationCode("location-2") && d.DisplayName == "Location 2");
        }

        [Fact]
        public void GetByCode_WithValidCode_ReturnsLocation()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetByCode(new LocationCode("location-2"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new LocationCode("location-2"), result.Code);
            Assert.Equal("Location 2", result.DisplayName);
        }

        [Fact]
        public void GetByCode_WithInvalidCode_ThrowsException()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => catalog.GetByCode(new LocationCode("invalid-code")));
        }

        [Fact]
        public void Constructor_WithNullSnapshot_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LocationDefinitionCatalog(null!));
        }

        private static LocationDefinitionCatalog CreateCatalog()
        {
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithLocation("location-2", "Location 2")
                .Build();

            return new LocationDefinitionCatalog(snapshot);
        }
    }
}
