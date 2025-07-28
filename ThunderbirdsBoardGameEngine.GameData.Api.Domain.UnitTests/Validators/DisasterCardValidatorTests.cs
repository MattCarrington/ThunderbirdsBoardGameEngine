using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Validators;
using ThunderbirdsBoardGameEngine.GameData.Api.TestHelpers.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.UnitTests.Validators
{
    public class DisasterCardValidatorTests
    {
        [Fact]
        public void Validate_WhenDisasterCardHasNullBonuses_ThrowsValidationException()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().WithNullBonusConditions().Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("must have at least one bonus", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardHasNoBonuses_ThrowsValidationException()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().WithoutBonusConditions().Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("must have at least one bonus", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardHasNullRewards_ThrowsValidationException()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().WithNullRewards().Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("must have at least one reward", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardHasNoRewards_ThrowsValidationException()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().WithoutRewards().Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("must have at least one reward option", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Validate_WhenDisasterCardHasInvalidDifficulty_ThrowsValidationException(int difficulty)
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().WithDifficulty(difficulty).Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("has invalid DifficultyNumber", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardRewardIsInvalid_ThrowsValidationException()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().WithUserChoiceRewardOption().Build();
            disasterCard.RewardOptions.First().IsUserChoice = false; // Simulate an invalid reward option

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("has an invalid reward option. If User Choice is false, specified token must be provided", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardHasDuplicateCharacterBonuses_ThrowsValidationException()
        {
            // Arrange
            var bonus = new CharacterBonusCondition()
            {
                Character = Character.Gordon,
                BonusValue = 2
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(bonus)
                .WithBonusCondition(bonus) // Duplicate bonus
                .Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("contains duplicate bonus", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardHasDuplicateThunderbirdBonuses_ThrowsValidationException()
        {
            // Arrange
            var bonus = new ThunderbirdBonusCondition()
            {
                Thunderbird = ThunderbirdMachine.Thunderbird3,
                BonusValue = 2
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(bonus)
                .WithBonusCondition(bonus) // Duplicate bonus
                .Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("contains duplicate bonus", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardHasDuplicatePodVehicleBonuses_ThrowsValidationException()
        {
            // Arrange
            var bonus = new PodVehicleBonusCondition()
            {
                PodVehicle = PodVehicle.TransmitterTruck,
                BonusValue = 2
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(bonus)
                .WithBonusCondition(bonus) // Duplicate bonus
                .Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("contains duplicate bonus", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Validate_WhenDisasterCardHasInvalidBonusValue_ThrowsValidationException(int bonusValue)
        {
            // Arrange
            var bonus = new CharacterBonusCondition
            {
                Character = Character.John,
                BonusValue = bonusValue // Using bonusValue to simulate invalid bonus value
            };

            var disasterCard = new DisasterCardBuilder().WithBonusCondition(bonus).Build();

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.Validate(disasterCard));
            Assert.Contains("has a bonus condition with invalid BonusValue", ex.Message);
        }

        [Fact]
        public void Validate_WhenDisasterCardIsValid_DoesNotThrow()
        {
            // Arrange
            var card = new DisasterCardBuilder().Build();

            // Act
            var exception = Record.Exception(() => DisasterCardValidator.Validate(card));

            // Assert
            Assert.Null(exception); // Means validation passed
        }

        [Fact]
        public void ValidateAll_WhenDisasterCardsAreValid_DoesNotThrow()
        {
            // Arrange
            var id = 1;

            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id++).Build(),
                new DisasterCardBuilder().WithId(id++).WithDifficulty(2).Build(),
                new DisasterCardBuilder().WithId(id).WithBonusCondition(new CharacterBonusCondition { Character = Character.Scott, BonusValue = 1 }).Build()
            };

            // Act
            var exception = Record.Exception(() => DisasterCardValidator.ValidateAll(cards));

            // Assert
            Assert.Null(exception); // Means validation passed
        }

        [Fact]
        public void ValidateAll_WhenDisasterCardsContainInvalidCard_ThrowsValidationException()
        {
            // Arrange
            var id = 1;
            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id++).Build(),
                new DisasterCardBuilder().WithId(id++).WithDifficulty(2).Build(),
                new DisasterCardBuilder().WithId(id).WithBonusCondition(new CharacterBonusCondition { Character = Character.Scott, BonusValue = 0 }).Build() // Invalid bonus value
            };

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains("has a bonus condition with invalid BonusValue", ex.Message);
        }

        [Fact]
        public void ValidateAll_WhenDuplicateDisasterCardIds_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(id).Build(),
                new DisasterCardBuilder().WithId(id).Build(),
                new DisasterCardBuilder().WithId(id).Build()
            };

            // Act & Assert
            var ex = Assert.Throws<DisasterCardValidationException>(() => DisasterCardValidator.ValidateAll(cards));
            Assert.Contains("Duplicate DisasterCard Id found", ex.Message);
        }
    }
}
