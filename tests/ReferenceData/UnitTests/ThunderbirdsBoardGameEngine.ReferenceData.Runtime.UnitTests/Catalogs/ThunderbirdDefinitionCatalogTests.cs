using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs
{
    public class ThunderbirdDefinitionCatalogTests
    {
        [Fact]
        public void GetByCode_WithValidCode_ReturnsThunderbird()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetByCode(new ThunderbirdCode("thunderbird-2"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new ThunderbirdCode("thunderbird-2"), result.Code);
            Assert.Equal("Thunderbird 2", result.DisplayName);
            Assert.Equal(MovementDomain.Space, result.Domain);
            Assert.Equal(3, result.TopSpeed);
        }

        [Fact]
        public void GetByCode_WithInvalidCode_ThrowsException()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => catalog.GetByCode(new ThunderbirdCode("invalid-code")));
        }

        [Fact]
        public void Constructor_WithNullSnapshot_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ThunderbirdDefinitionCatalog(null!));
        }

        private static ThunderbirdDefinitionCatalog CreateCatalog()
        {
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithThunderbird("thunderbird-1", "Thunderbird 1")
                .WithThunderbird("thunderbird-2", "Thunderbird 2", MovementDomain.Space, 3)
                .Build();

            return new ThunderbirdDefinitionCatalog(snapshot);
        }
    }
}
