using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Rescue
{
    public class RescueTargetCalculatorTests
    {
        private readonly DisasterContribution _contribution = new(
            DifficultyNumber: 9,
            Bonuses: new List<DisasterBonus>
            {
                new DisasterBonus("BONUS_1", 2),
                new DisasterBonus("BONUS_2", 3),
                new DisasterBonus("BONUS_X", 5)
            });

        [Fact]
        public void CalculateRescueTarget_NoBonusesToApply_ReturnsTargetRollEqualDifficultyNumber()
        {
            // Arrange
            var calculator = CreateResuceTargetCalculator();

            var appliedBonusKeys = Array.Empty<string>();

            // Act
            var result = calculator.CalculateRescueTarget(appliedBonusKeys, _contribution);

            // Assert
            Assert.Equal(9, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedBonuses);
        }

        [Fact]
        public void CalculateRescueTarget_WhenBonusesToBeApplied_ReturnsTargetRoll()
        {
            // Arrange
            var calculator = CreateResuceTargetCalculator();

            var bonusKeys = new List<string>()
            {
                "BONUS_1",
                "BONUS_2"
            };

            // Act
            var result = calculator.CalculateRescueTarget(bonusKeys, _contribution);

            // Assert
            Assert.Equal(4, result.TargetRoll);
            Assert.Equal(5, result.TotalBonus);

            var expectedBonuses = new[]
            {
                new DisasterBonus("BONUS_1", 2),
                new DisasterBonus("BONUS_2", 3)
            };

            Assert.Equal(expectedBonuses.Length, result.AppliedBonuses.Count);
            Assert.All(expectedBonuses, expected =>
                Assert.Contains(expected, result.AppliedBonuses));
        }

        [Fact]
        public void CalculateRescueResult_WhenInvalidBonusKey_ReturnsTargetRoll()
        {
            // Arrange
            var calculator = CreateResuceTargetCalculator();

            var bonusKeys = new List<string>()
            {
                "BONUS_3",
                "BONUS_4"
            };

            // Act
            var result = calculator.CalculateRescueTarget(bonusKeys, _contribution);

            // Assert
            Assert.Equal(9, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedBonuses);
        }

        private static RescueTargetCalculator CreateResuceTargetCalculator()
        {
            return new RescueTargetCalculator();
        }
    }
}
