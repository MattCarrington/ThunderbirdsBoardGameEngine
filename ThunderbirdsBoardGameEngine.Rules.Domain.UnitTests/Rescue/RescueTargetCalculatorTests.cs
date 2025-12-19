using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Rescue
{
    public class RescueTargetCalculatorTests
    {
        private readonly RescueProjection _rescueContext = new(
            DifficultyNumber: 9,
            Bonuses: new List<RescueBonus>
            {
                new RescueBonus("BONUS_1", 2),
                new RescueBonus("BONUS_2", 3),
                new RescueBonus("BONUS_X", 5)
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
            Assert.Empty(result.AppliedBonuses);
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

            var expectedBonuses = new[]
            {
                new RescueBonus("BONUS_1", 2),
                new RescueBonus("BONUS_2", 3)
            };

            Assert.Equal(expectedBonuses.Length, result.AppliedBonuses.Count);
            Assert.All(expectedBonuses, expected =>
                Assert.Contains(expected, result.AppliedBonuses));
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
            Assert.Empty(result.AppliedBonuses);
        }

        private static RescueTargetCalculator CreateBonusCalculator()
        {
            return new RescueTargetCalculator();
        }
    }
}
