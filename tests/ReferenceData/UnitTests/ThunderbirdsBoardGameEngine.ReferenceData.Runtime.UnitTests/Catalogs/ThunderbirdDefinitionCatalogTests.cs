using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs
{
    public class ThunderbirdDefinitionCatalogTests
    {
        [Fact]
        public void Constructor_WithNullSnapshot_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ThunderbirdDefinitionCatalog(null!));
        }

        [Fact]
        public void TryGetByCode_WithExistingCode_ReturnsTrueAndDefinition()
        {
            // Arrange
            var catalog = CreateCatalog();

            var code = new ThunderbirdCode("thunderbird-2");

            // Act
            var result = catalog.TryGetByCode(code, out var definition);

            // Assert
            Assert.True(result);
            Assert.NotNull(definition);
            Assert.Equal(code, definition!.Code);
            Assert.Equal("Thunderbird 2", definition.DisplayName);
            Assert.Equal(MovementDomain.Space, definition.Domain);
            Assert.Equal(3, definition.TopSpeed);
        }

        [Fact]
        public void TryGetByCode_WithNonExistingCode_ReturnsFalseAndNull()
        {
            // Arrange
            var catalog = CreateCatalog();

            var code = new ThunderbirdCode("non-existing-code");

            // Act
            var result = catalog.TryGetByCode(code, out var definition);

            // Assert
            Assert.False(result);
            Assert.Null(definition);
        }

        [Fact]
        public void GetAll_WhenCalled_ReturnsAllThunderbirds()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetAll();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Contains(result, t => t.Code == new ThunderbirdCode("thunderbird-1") && t.DisplayName == "Thunderbird 1" && t.Domain == MovementDomain.Earth && t.TopSpeed == 0);
            Assert.Contains(result, t => t.Code == new ThunderbirdCode("thunderbird-2") && t.DisplayName == "Thunderbird 2" && t.Domain == MovementDomain.Space && t.TopSpeed == 3);
        }

        [Fact]
        public void GetByCode_WithExistingCode_ReturnsDefinition()
        {
            // Arrange
            var catalog = CreateCatalog();

            var code = new ThunderbirdCode("thunderbird-1");
            // Act
            var definition = catalog.GetByCode(code);

            // Assert
            Assert.NotNull(definition);
            Assert.Equal(code, definition.Code);
            Assert.Equal("Thunderbird 1", definition.DisplayName);
            Assert.Equal(MovementDomain.Earth, definition.Domain);
            Assert.Equal(0, definition.TopSpeed);
        }

        [Fact]
        public void GetByCode_WithNonExistingCode_ThrowsKeyNotFoundException()
        {
            // Arrange
            var catalog = CreateCatalog();

            var code = new ThunderbirdCode("non-existing-code");

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => catalog.GetByCode(code));
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
