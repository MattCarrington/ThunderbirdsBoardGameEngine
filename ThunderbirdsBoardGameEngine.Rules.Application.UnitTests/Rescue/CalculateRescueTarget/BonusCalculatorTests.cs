using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.Rescue.CalculateRescueTarget
{
    public class BonusCalculatorTests
    {
        private readonly List<RuleBonus> _cardBonuses = new()
        {
            new() { Key = "BONUS_1", Value = 2 },
            new() { Key = "BONUS_2", Value = 3 }
        };

        [Fact]
        public void CalculateRescueTarget_NoBonusesToApply_ReturnsTargetRollEqualDifficultyNumber()
        {
            // Arrange
            var bonusCalculator = CreateBonusCalculator();

            var difficultyNumber = 5;
            var appliedBonusKeys = Array.Empty<string>();

            // Act
            var result = bonusCalculator.CalculateRescueTarget(difficultyNumber, appliedBonusKeys, _cardBonuses);

            // Assert
            Assert.Equal(difficultyNumber, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
        }

        [Fact]
        public void CalculateRescueTarget_WhenBonusesToBeApplied_ReturnsTargetRoll()
        {
            // Arrange
            var bonusCalculator = CreateBonusCalculator();

            var difficultyNumber = 9;

            var bonusKeys = new List<string>()
            {
                "BONUS_1",
                "BONUS_2"
            };

            // Act
            var result = bonusCalculator.CalculateRescueTarget(difficultyNumber, bonusKeys, _cardBonuses);

            // Assert
            Assert.Equal(4, result.TargetRoll);
            Assert.Equal(5, result.TotalBonus);
        }

        [Fact]
        public void CalculateRescueResult_WhenInvalidBonusKey_ReturnsTargetRoll()
        {
            // Arrange
            var bonusCalculator = CreateBonusCalculator();

            var difficultyNumber = 9;

            var bonusKeys = new List<string>()
            {
                "BONUS_3",
                "BONUS_4"
            };

            // Act
            var result = bonusCalculator.CalculateRescueTarget(difficultyNumber, bonusKeys, _cardBonuses);

            // Assert
            Assert.Equal(9, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
        }

        private static BonusCalculator CreateBonusCalculator()
        {
            return new BonusCalculator();
        }
    }
}
