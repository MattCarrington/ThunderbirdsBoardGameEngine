using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Validators
{
    public class CharacterDefinitionValidatorTests
    {
        [Fact]
        public void ValidateAll_WhenCharacterDefinitionsAreValid_DoesNotThrow()
        {
            // Arrange
            var characters = new List<CharacterDefinition>
            {
                new(Character.Scott, new(RescueType.Air, 2)),
                new(Character.Virgil, new(RescueType.Land, 2)),
                new(Character.John, new(RescueType.Space, 2)),
                new(Character.Gordon, new(RescueType.Sea, 3)),
                new(Character.Alan, new(RescueType.Space, 2)),
                new(Character.LadyPenelope)
            };

            // Act
            var exception = Record.Exception(() => CharacterDefinitionValidator.ValidateAll(characters));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateAll_WhenDuplicateKeys_ThrowsValidationException()
        {
            var duplicateCharacter = Character.Scott;

            // Arrange
            var characters = new List<CharacterDefinition>
            {
                new(duplicateCharacter),
                new(duplicateCharacter, new(RescueType.Air, 2)),
                new(Character.Virgil, new(RescueType.Land, 2)),
                new(Character.John, new(RescueType.Space, 2)),
                new(Character.Gordon, new(RescueType.Sea, 3)),
                new(Character.Alan, new(RescueType.Space, 2)),
            };
            
            // Act & Assert
            var exception = Assert.Throws<CharacterDefinitionValidationException>(() => CharacterDefinitionValidator.ValidateAll(characters));
            Assert.Contains($"A character definition with the key '{duplicateCharacter}' already exists.", exception.Message);
            Assert.Equal(CharacterDefinitionErrorCode.DuplicateKey, exception.ErrorCode);
            Assert.Equal(duplicateCharacter, exception.Character);
        }

        [Fact]
        public void ValidateAll_WhenCharacterDefiinitionsLessThanSix_ThrowsValidationException()
        {
            // Arrange
            var characters = new List<CharacterDefinition>
            {
                new(Character.Scott, new(RescueType.Air, 2)),
                new(Character.Virgil, new(RescueType.Land, 2)),
                new(Character.John, new(RescueType.Space, 2)),
                new(Character.Gordon, new(RescueType.Sea, 3)),
                new(Character.Alan, new(RescueType.Space, 2))
            };

            // Act & Assert
            var exception = Assert.Throws<CharacterDefinitionValidationException>(() => CharacterDefinitionValidator.ValidateAll(characters));
            Assert.Contains("The number of character definitions must be exactly six.", exception.Message);
        }



        [Fact]
        public void ValidateAll_WhenCharacterDefiinitionsGreaterThanSix_ThrowsValidationException()
        {
            // Arrange
            var characters = new List<CharacterDefinition>
            {
                new(Character.Scott, new(RescueType.Air, 2)),
                new(Character.Virgil, new(RescueType.Land, 2)),
                new(Character.John, new(RescueType.Space, 2)),
                new(Character.Gordon, new(RescueType.Sea, 3)),
                new(Character.Alan, new(RescueType.Space, 2)),
                new(Character.LadyPenelope),
                new(Character.LadyPenelope),
            };

            // Act & Assert
            var exception = Assert.Throws<CharacterDefinitionValidationException>(() => CharacterDefinitionValidator.ValidateAll(characters));
            Assert.Contains("The number of character definitions must be exactly six.", exception.Message);
        }

        [Fact]
        public void ValidateAll_WhenCharacterDefinitionsAreNull_ThrowsArgumentNullException()
        {
            // Arrange
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => CharacterDefinitionValidator.ValidateAll(null!));
            Assert.Contains("Value cannot be null.", exception.Message);
        }
    }
}
