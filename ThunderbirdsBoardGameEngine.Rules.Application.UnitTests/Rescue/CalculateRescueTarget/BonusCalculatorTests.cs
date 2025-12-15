using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.Rescue.CalculateRescueTarget
{
    public class BonusCalculatorTests
    {
        private readonly RescueContext _rescueContext = new(
            DifficultyNumber: 9,
            Bonuses: new List<RescueContextBonus>
            {
                new RescueContextBonus("BONUS_1", 2),
                new RescueContextBonus("BONUS_2", 3)
            });

        [Fact]
        public void CalculateRescueTarget_NoBonusesToApply_ReturnsTargetRollEqualDifficultyNumber()
        {
            // Arrange
            var bonusCalculator = CreateBonusCalculator();

            var appliedBonusKeys = Array.Empty<string>();

            // Act
            var result = bonusCalculator.CalculateRescueTarget(appliedBonusKeys, _rescueContext);

            // Assert
            Assert.Equal(9, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
        }

        [Fact]
        public void CalculateRescueTarget_WhenBonusesToBeApplied_ReturnsTargetRoll()
        {
            // Arrange
            var bonusCalculator = CreateBonusCalculator();

            var bonusKeys = new List<string>()
            {
                "BONUS_1",
                "BONUS_2"
            };

            // Act
            var result = bonusCalculator.CalculateRescueTarget(bonusKeys, _rescueContext);

            // Assert
            Assert.Equal(4, result.TargetRoll);
            Assert.Equal(5, result.TotalBonus);
        }

        [Fact]
        public void CalculateRescueResult_WhenInvalidBonusKey_ReturnsTargetRoll()
        {
            // Arrange
            var bonusCalculator = CreateBonusCalculator();

            var bonusKeys = new List<string>()
            {
                "BONUS_3",
                "BONUS_4"
            };

            // Act
            var result = bonusCalculator.CalculateRescueTarget(bonusKeys, _rescueContext);

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
