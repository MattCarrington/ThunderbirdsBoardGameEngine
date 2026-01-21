using ThunderbirdsBoardGameEngine.Api.Mappers.Catalog.V1;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Mappers.Catalog.V1
{
    public class CharacterDefinitionMappingExtensionsTests
    {
        [Fact]
        public void ToDto_WhenValid_ShouldMapCharacterDto()
        {
            // Arrange
            var character = new CharacterDefinition(Character.LadyPenelope, null);

            // Act
            var result = character.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("LadyPenelope", result.Key);
            Assert.Equal("Lady Penelope", result.DisplayName);
        }

        [Fact]
        public void ToDto_WhenListOfCharacterDefinitions_ShouldMapAll()
        {
            // Arrange
            var characters = TestCharacters.ValidSix;

            // Act
            var result = characters.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(characters.Count, result.Count);
        }
    }
}
