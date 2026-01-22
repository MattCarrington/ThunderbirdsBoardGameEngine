using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
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
        private const string _version = "1.0";

        [Fact]
        public void Constructor_WithValidParameters_SetsProperties()
        {
            // Arrange
            var characters = TestCharacters.ValidSix.ToImmutableArray();


            // Act
            var source = new InMemoryCharacterDefinitionReferenceSource(characters, _version);

            // Assert
            Assert.Equal(_version, source.Version);
            Assert.Equal(characters, source.Characters);
        }

        [Fact]
        public void Constructor_WithEmptyCharacters_ThrowsArgumentException()
        {
            // Arrange
            var emptyCharacters = ImmutableArray<CharacterDefinition>.Empty;
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryCharacterDefinitionReferenceSource(emptyCharacters, _version));
        }

        [Fact]
        public void Constructor_WithDefaultCharacters_ThrowsArgumentException()
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryCharacterDefinitionReferenceSource(default, _version));
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

        [Fact]
        public void GetCharacterDefinition_WithValidCharacter_ReturnsDefinition()
        {
            // Arrange
            var source = CreateReferenceSource();

            // Act
            var result = source.GetCharacterDefinition(Character.Virgil);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Character.Virgil, result.Key);
            Assert.NotNull(result.RescueBonus);
            Assert.Equal(RescueType.Land, result.RescueBonus.RescueType);
            Assert.Equal(2, result.RescueBonus.BonusValue);
        }

        [Fact]
        public void GetCharacterDefinition_WithMissingCharacter_ThrowsCharacterDefinitionNotFoundException()
        {
            // Arrange
            var excludedCharacter = Character.LadyPenelope;

            var characters = TestCharacters.ValidSix.Where(c => c.Key != excludedCharacter).ToImmutableArray();

            var source = CreateReferenceSource(characters);

            // Act & Assert
            Assert.Throws<CharacterDefinitionNotFoundException>(() => source.GetCharacterDefinition(excludedCharacter));
        }

        private static InMemoryCharacterDefinitionReferenceSource CreateReferenceSource()
        {
            var characters = TestCharacters.ValidSix.ToImmutableArray();

            return CreateReferenceSource(characters);
        }

        private static InMemoryCharacterDefinitionReferenceSource CreateReferenceSource(ImmutableArray<CharacterDefinition> characters)
        {
            return new InMemoryCharacterDefinitionReferenceSource(characters, _version);
        }
    }
}
