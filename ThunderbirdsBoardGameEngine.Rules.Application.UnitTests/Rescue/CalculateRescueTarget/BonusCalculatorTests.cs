using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.Rescue.CalculateRescueTarget
{
    public class BonusCalculatorTests
    {
        [Fact]
        public void CalculateRescueTarget_NoBonusesToApply_ReturnsTargetRollEqualDifficultyNumber()
        {
            // Arrange
            var bonusCalculator = new BonusCalculator();

            var difficultyNumber = 5;
            var appliedBonusKeys = Array.Empty<string>();

            var cardBonuses = new List<RuleBonus>
            {
                new() { Key = "BONUS_1", Value = 2 },
                new() { Key = "BONUS_2", Value = 3 }
            };

            // Act
            var result = bonusCalculator.CalculateRescueTarget(difficultyNumber, appliedBonusKeys, cardBonuses);

            // Assert
            Assert.Equal(difficultyNumber, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
        }

        [Fact]
        public void CalculateRescueTarget_WhenBonusesToBeApplied_ReturnsTargetRoll()
        {
            // Arrange
            var bonusCalculator = new BonusCalculator();

            var difficultyNumber = 9;

            var bonusKeys = new List<string>()
            {
                "BONUS_1",
                "BONUS_2"
            };

            var cardBonuses = new List<RuleBonus>
            {
                new() { Key = "BONUS_1", Value = 2 },
                new() { Key = "BONUS_2", Value = 3 }
            };

            // Act
            var result = bonusCalculator.CalculateRescueTarget(difficultyNumber, bonusKeys, cardBonuses);

            // Assert
            Assert.Equal(4, result.TargetRoll);
            Assert.Equal(5, result.TotalBonus);
        }

        [Fact]
        public void CalculateRescueResult_WhenInvalidBonusKey_ReturnsTargetRoll()
        {
            // Arrange
            var bonusCalculator = new BonusCalculator();

            var difficultyNumber = 9;

            var bonusKeys = new List<string>()
            {
                "BONUS_3",
                "BONUS_4"
            };

            var cardBonuses = new List<RuleBonus>
            {
                new() { Key = "BONUS_1", Value = 2 },
                new() { Key = "BONUS_2", Value = 3 }
            };

            // Act
            var result = bonusCalculator.CalculateRescueTarget(difficultyNumber, bonusKeys, cardBonuses);

            // Assert
            Assert.Equal(9, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
        }
    }
}
