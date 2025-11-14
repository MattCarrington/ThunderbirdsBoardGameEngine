using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
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
                new DisasterCardBuilder().WithId(id).WithName($"Test {id}").WithCode($"test-{id++}").Build(),
                new DisasterCardBuilder().WithId(id).WithName($"Test {id}").WithDifficulty(2).WithCode($"test-{id++}").Build(),
                new DisasterCardBuilder().WithId(id).WithName($"Test {id}").WithBonusCondition(new CharacterBonusCondition(Character.Scott, 1)).WithCode($"test-{id}").Build()
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
                new DisasterCardBuilder().WithId(id).WithName("Test 1").WithCode("Test-1").Build(),
                new DisasterCardBuilder().WithId(id).WithName("Test 2").WithCode("Test-2").Build(),
                new DisasterCardBuilder().WithId(id).WithName("Test 3").WithCode("Test-3").Build()
            };

            // Act & Assert
            var exception = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains($"A disaster card with the id '{id}' already exists.", exception.Message);
            Assert.Equal(DisasterCardErrorCode.DuplicateId, exception.ErrorCode);
            Assert.Equal(id, exception.CardId);
        }

        [Theory]
        [InlineData("duplicate name")]
        [InlineData("DUPLICATE NAME")]
        [InlineData("Duplicate Name")]
        public void ValidateAll_WhenDuplicateDisasterCardNames_ThrowsValidationException(string name)
        {
            // Arrange
            var id = 1;

            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id).WithName("Duplicate Name").WithCode($"test-{id++}").Build(),
                new DisasterCardBuilder().WithId(id).WithName(name).WithCode($"test-{id}").Build()
            };

            // Act & Assert
            var exception = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains($"A disaster card with the Name '{name}' already exists.", exception.Message);
            Assert.Equal(DisasterCardErrorCode.DuplicateName, exception.ErrorCode);
            Assert.Equal(name, exception.CardName);
        }

        [Theory]
        [InlineData("duplicate-code")]
        [InlineData("DUPLICATE-CODE")]
        [InlineData("Duplicate-Code")]
        public void ValidateAll_WhenDuplicateDisasterCardCodes_ThrowsValidationException(string code)
        {
            // Arrange
            var id = 1;
            
            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id).WithName($"Test {id++}").WithCode("duplicate-code").Build(),
                new DisasterCardBuilder().WithId(id).WithName($"Test {id}").WithCode(code).Build()
            };
            // Act & Assert
            var exception = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains($"A disaster card with the Code '{code.ToLower()}' already exists.", exception.Message);
            Assert.Equal(DisasterCardErrorCode.DuplicateCode, exception.ErrorCode);
            Assert.Equal(id, exception.CardId);
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
            Assert.Contains("The disaster cards collection contains a null entry.", exception.Message);
        }

        [Fact]
        public void ValidateAll_WhenDisasterCardsNull_ThrowsValidationException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => DisasterCardValidator.ValidateAll(null!));
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
