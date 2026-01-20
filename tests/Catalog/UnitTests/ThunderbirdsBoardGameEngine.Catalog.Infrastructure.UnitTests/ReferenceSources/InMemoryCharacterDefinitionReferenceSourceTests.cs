using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.ReferenceSources
{
    public class InMemoryCharacterDefinitionReferenceSourceTests
    {
        [Fact]
        public void Constructor_WithValidParameters_SetsProperties()
        {
            // Arrange
            var characters = TestCharacters.ValidSix.ToImmutableArray();
            var version = "1.0";

            // Act
            var source = new InMemoryCharacterDefinitionReferenceSource(characters, version);

            // Assert
            Assert.Equal(version, source.Version);
            Assert.Equal(characters, source.Characters);
        }

        [Fact]
        public void Constructor_WithEmptyCharacters_ThrowsArgumentException()
        {             
            // Arrange
            var emptyCharacters = ImmutableArray<CharacterDefinition>.Empty;
            var version = "1.0";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryCharacterDefinitionReferenceSource(emptyCharacters, version));
        }

        [Fact]
        public void Constructor_WithDefaultCharacters_ThrowsArgumentException()
        {
            // Arrange
            var version = "1.0";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryCharacterDefinitionReferenceSource(default, version));
        }

        [Fact]
        public void Constructor_WithNullVersion_ThrowsArgumentNullException()
        {
            // Arrange
            var characters = TestCharacters.ValidSix.ToImmutableArray();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InMemoryCharacterDefinitionReferenceSource(characters, null!));
        }

        [Theory]
        [ClassData(typeof(WhiteSpaceStringData))]
        public void Constructor_WithWhiteSpaceVersion_ThrowsArgumentException(string version)
        {
            // Arrange
            var characters = TestCharacters.ValidSix.ToImmutableArray();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryCharacterDefinitionReferenceSource(characters, version));
        }
    }
}
