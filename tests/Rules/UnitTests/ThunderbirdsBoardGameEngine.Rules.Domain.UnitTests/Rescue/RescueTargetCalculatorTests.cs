using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
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
                new(new DisasterBonusKey("BONUS_1"), 2),
                new(new DisasterBonusKey("BONUS_2"), 3),
                new(new DisasterBonusKey("BONUS_X"), 5)
            });

        [Fact]
        public void CalculateRescueTarget_NoBonusesToApply_ReturnsTargetRollEqualDifficultyNumber()
        {
            // Arrange
            var calculator = CreateRescueTargetCalculator();

            var appliedBonusKeys = Array.Empty<DisasterBonusKey>();

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
            var calculator = CreateRescueTargetCalculator();

            var bonusKeys = new List<DisasterBonusKey>()
            {
                new("BONUS_1"),
                new("BONUS_2")
            };

            // Act
            var result = calculator.CalculateRescueTarget(bonusKeys, _contribution);

            // Assert
            Assert.Equal(4, result.TargetRoll);
            Assert.Equal(5, result.TotalBonus);

            var expectedBonuses = new[]
            {
                new DisasterBonus(new DisasterBonusKey("BONUS_1"), 2),
                new DisasterBonus(new DisasterBonusKey("BONUS_2"), 3)
            };

            Assert.Equal(expectedBonuses.Length, result.AppliedBonuses.Count);
            Assert.All(expectedBonuses, expected =>
                Assert.Contains(expected, result.AppliedBonuses));
        }

        [Fact]
        public void CalculateRescueTarget_WhenInvalidBonusKey_ReturnsTargetRoll()
        {
            // Arrange
            var calculator = CreateRescueTargetCalculator();

            var bonusKeys = new List<DisasterBonusKey>()
            {
                new("BONUS_3"),
                new("BONUS_4")
            };

            // Act
            var result = calculator.CalculateRescueTarget(bonusKeys, _contribution);

            // Assert
            Assert.Equal(9, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedBonuses);
        }

        [Fact]
        public void CalculateRescueTarget_WhenDuplicateBonusKey_KeyAppliedOnce()
        {
            // Arrange
            var calculator = CreateRescueTargetCalculator();

            var duplicate = new DisasterBonusKey("BONUS_1");

            var bonusKeys = new List<DisasterBonusKey>
            {
                duplicate,
                duplicate
            };

            // Act
            var result = calculator.CalculateRescueTarget(bonusKeys, _contribution);

            // Assert
            Assert.Equal(7, result.TargetRoll);
            Assert.Equal(2, result.TotalBonus);
            
            var bonus = Assert.Single<DisasterBonus>(result.AppliedBonuses);
            
            Assert.Equal(duplicate, bonus.Key);
            Assert.Equal(2, bonus.Value);
        }

        private static RescueTargetCalculator CreateRescueTargetCalculator()
        {
            return new RescueTargetCalculator();
        }
    }
}
