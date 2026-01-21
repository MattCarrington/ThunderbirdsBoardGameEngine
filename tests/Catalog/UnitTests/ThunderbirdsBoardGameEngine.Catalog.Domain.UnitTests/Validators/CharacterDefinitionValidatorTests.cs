using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Validators
{
    public class CharacterDefinitionValidatorTests
    {
        [Fact]
        public void ValidateAll_WhenCharacterDefinitionsAreValid_DoesNotThrow()
        {
            // Arrange
            var characters = TestCharacters.ValidSix;

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
            var characters = TestCharacters.ValidSix
                .Where(c => c.Key != Character.LadyPenelope)
                .ToList();

            characters.Add(new CharacterDefinition(duplicateCharacter));

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
            var characters = TestCharacters.ValidSix.Take(5).ToList();

            // Act & Assert
            var exception = Assert.Throws<CharacterDefinitionValidationException>(() => CharacterDefinitionValidator.ValidateAll(characters));
            Assert.Contains("The number of character definitions must be exactly six.", exception.Message);
        }

        [Fact]
        public void ValidateAll_WhenCharacterDefiinitionsGreaterThanSix_ThrowsValidationException()
        {
            // Arrange
            var characters = TestCharacters.ValidSix.ToList();
            characters.Add(new CharacterDefinition(Character.LadyPenelope));

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
