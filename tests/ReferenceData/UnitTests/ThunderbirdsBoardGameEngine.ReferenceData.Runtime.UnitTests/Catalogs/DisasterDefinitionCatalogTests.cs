using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs
{
    public class DisasterDefinitionCatalogTests
    {
        [Fact]
        public void GetAll_WithValidSnapshot_ReturnsAllDisasters()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetAll();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Contains(result, d => d.Code == new CardCode("disaster-1") && d.DisplayName == "Disaster 1");
            Assert.Contains(result, d => d.Code == new CardCode("disaster-2") && d.DisplayName == "Disaster 2");
        }

        [Fact]
        public void GetByCode_WithValidCode_ReturnsDisaster()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetByCode(new CardCode("disaster-2"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new CardCode("disaster-2"), result.Code);
            Assert.Equal("Disaster 2", result.DisplayName);
        }

        [Fact]
        public void GetByCode_WithInvalidCode_ThrowsException()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => catalog.GetByCode(new CardCode("invalid-code")));
        }

        [Fact]
        public void Constructor_WithNullSnapshot_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DisasterDefinitionCatalog(null!));
        }

        private static DisasterDefinitionCatalog CreateCatalog()
        {
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("character-1", 1, null))
                .WithDisaster("disaster-2", "Disaster 2", "location-1", ("character-1", 2, null))
                .Build();

            return new DisasterDefinitionCatalog(snapshot);
        }
    }

    public class DisasterBonusKeyDefinitionCatalogTests
    {
        [Fact]
        public void GetByCode_WithValidCharacterCode_ReturnsDisasterBonusKey()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetByCode(new DisasterBonusKey("character-1"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new DisasterBonusKey("character-1"), result.Key);
            Assert.Equal("Character 1", result.DisplayName);
        }

        [Fact]
        public void GetByCode_WithValidThunderbirdCode_ReturnsDisasterBonusKey()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetByCode(new DisasterBonusKey("thunderbird-1"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new DisasterBonusKey("thunderbird-1"), result.Key);
            Assert.Equal("Thunderbird 1", result.DisplayName);
        }

        [Fact]
        public void GetByCode_WithValidPodVehicleCode_ReturnsDisasterBonusKey()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.GetByCode(new DisasterBonusKey("pod-1"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new DisasterBonusKey("pod-1"), result.Key);
            Assert.Equal("Pod 1", result.DisplayName);
        }

        [Fact]
        public void GetByCode_WithInvalidCode_ThrowsException()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => catalog.GetByCode(new DisasterBonusKey("invalid-code")));
        }

        [Fact]
        public void Constructor_WithNullSnapshot_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DisasterBonusKeyDefinitionCatalog(null!));
        }

        private static DisasterBonusKeyDefinitionCatalog CreateCatalog()
        {
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("character-1", 1, null))
                .WithDisaster("disaster-2", "Disaster 2", "location-1", ("character-1", 2, null))
                .WithThunderbird("thunderbird-1", "Thunderbird 1")
                .WithPodVehicle("pod-1", "Pod 1")
                .Build();

            return new DisasterBonusKeyDefinitionCatalog(snapshot);
        }
    }
}
