using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Rescue
{
    public class DisasterContributionTests
    {
        [Fact]
        public void ApplyRescueModifier_NoBonusesToApply_ReturnsEmpty()
        {
            // Arrange
            var contribution = CreateDisasterContribution();

            var input = CreateInput(Array.Empty<DisasterBonusKey>());

            // Act
            var result = contribution.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ApplyRescueModifier_WhenBonusesToBeApplied_ReturnsAppliedBonuses()
        {
            // Arrange
            var contribution = CreateDisasterContribution();

            var bonusKeys = new List<DisasterBonusKey>()
            {
                new("BONUS_1"),
                new("BONUS_2")
            };

            var input = CreateInput(bonusKeys);

            // Act
            var result = contribution.ApplyRescueModifier(input);

            // Assert
            var expectedBonuses = new List<AppliedRescueModifier>
            {
                new() {
                    Key = "BONUS_1",
                    Value = 2,
                    SourceType = SourceType.DisasterCard
                },
                new() {
                    Key = "BONUS_2",
                    Value = 3,
                    SourceType = SourceType.DisasterCard
                }
            };

            Assert.Equal(expectedBonuses.Count, result.ToList().Count);
            Assert.All(expectedBonuses, expected =>
                Assert.Contains(expected, result));
        }

        [Fact]
        public void ApplyRescueModifier_WhenInvalidBonusKey_ReturnsEmpty()
        {
            // Arrange
            var contribution = CreateDisasterContribution();

            var bonusKeys = new List<DisasterBonusKey>()
            {
                new("BONUS_3"),
                new("BONUS_4")
            };

            var input = CreateInput(bonusKeys);

            // Act
            var result = contribution.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ApplyRescueModifier_WhenDuplicateBonusKey_KeyAppliedOnce()
        {
            // Arrange
            var contribution = CreateDisasterContribution();

            var duplicate = new DisasterBonusKey("BONUS_1");

            var bonusKeys = new List<DisasterBonusKey>
            {
                duplicate,
                duplicate
            };

            var input = CreateInput(bonusKeys);

            // Act
            var result = contribution.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single<AppliedRescueModifier>(result);

            Assert.Equal(duplicate.ToString(), bonus.Key);
            Assert.Equal(2, bonus.Value);
            Assert.Equal(SourceType.DisasterCard, bonus.SourceType);
        }

        private static DisasterContribution CreateDisasterContribution()
        {
            return new(
            difficultyNumber: 9,
            availableBonuses:
            [
                new(new DisasterBonusKey("BONUS_1"), 2),
                new(new DisasterBonusKey("BONUS_2"), 3),
                new(new DisasterBonusKey("BONUS_X"), 5)
            ],
            rescueType: RescueType.Land);
        }

        private static RescueCalculationInput CreateInput(IReadOnlyCollection<DisasterBonusKey> bonusKeys)
        {
            return new RescueCalculationInput(bonusKeys, RescueType.Sea);
        }
    }
}
