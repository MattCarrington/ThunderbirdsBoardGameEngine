using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Validators
{
    public class DisasterCardValidatorTests
    {
        [Fact]
        public void ValidateAll_WhenDisasterCardsAreValid_DoesNotThrow()
        {
            // Arrange
            var id = 1;

            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id++).WithName($"Test 1").Build(),
                new DisasterCardBuilder().WithId(id++).WithName($"Test 2").WithDifficulty(2).Build(),
                new DisasterCardBuilder().WithId(id).WithName($"Test 3").WithBonusCondition(new CharacterBonusCondition(Character.Scott, 1)).Build()
            };

            // Act
            var exception = Record.Exception(() => DisasterCardValidator.ValidateAll(cards));

            // Assert
            Assert.Null(exception); // Means validation passed
        }

        [Fact]
        public void ValidateAll_WhenDuplicateDisasterCardIds_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id).WithName("Test 1").Build(),
                new DisasterCardBuilder().WithId(id).WithName("Test 2").Build(),
                new DisasterCardBuilder().WithId(id).WithName("Test 3").Build()
            };

            // Act & Assert
            var exception = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains("Duplicate Disaster Card Id found", exception.Message);
        }

        [Theory]
        [InlineData("duplicate name")]
        [InlineData("DUPLICATE NAME")]
        public void ValidateAll_WhenDuplicateDisasterCardNames_ThrowsValidationException(string name)
        {
            // Arrange
            var id = 1;

            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id++).WithName("Duplicate Name").Build(),
                new DisasterCardBuilder().WithId(id).WithName(name).Build()
            };

            // Act & Assert
            var exception = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains("Duplicate Disaster Card Name found", exception.Message);
        }

        [Fact]
        public void ValidateAll_WhenDisasterCardIsNull_ThrowsValidationException()
        {
            // Arrange
            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Test 1").Build(),
                null!,
                new DisasterCardBuilder().WithId(2).WithName("Test 2").Build()
            };

            // Act & Assert
            var exception = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains("Disaster Cards collection contains null entries.", exception.Message);
        }

        [Fact]
        public void ValidateAll_WhenEmptyDisasterCardList_DoesNotThrow()
        {
            // Arrange
            var cards = new List<DisasterCard>();

            // Act
            var exception = Record.Exception(() => DisasterCardValidator.ValidateAll(cards));

            // Assert
            Assert.Null(exception); // Means validation passed
        }
    }
}
