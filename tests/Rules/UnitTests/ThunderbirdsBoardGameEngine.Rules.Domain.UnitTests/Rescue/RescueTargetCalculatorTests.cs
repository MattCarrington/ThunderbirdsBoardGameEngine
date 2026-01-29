using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Rescue
{
    public class RescueTargetCalculatorTests
    {
        [Fact]
        public void CalculateRescueTarget_NoBonusSources_ReturnsBaseDifficultyAsTarget()
        {
            // Arrange
            var calculator = new RescueTargetCalculator();

            var difficultyNumber = 7;

            var input = CreateInput();

            var bonusSources = Array.Empty<IBonusModifierSource>();

            // Act
            var result = calculator.CalculateRescueTarget(difficultyNumber, input, bonusSources);

            // Assert
            Assert.Equal(difficultyNumber, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedBonuses);
        }

        [Fact]
        public void CalculateRescueTarget_WithBonusSources_ReturnsModifiedTarget()
        {
            // Arrange
            var calculator = new RescueTargetCalculator();

            var difficultyNumber = 10;

            var input = CreateInput();

            var bonusSources = new IBonusModifierSource[]
            {
                new FakeSource(2, 3),
                new FakeSource(1)
            };

            // Act
            var result = calculator.CalculateRescueTarget(difficultyNumber, input, bonusSources);

            // Assert
            var expectedTotalBonus = 6;
            var expectedTarget = difficultyNumber - expectedTotalBonus;

            Assert.Equal(expectedTarget, result.TargetRoll);
            Assert.Equal(expectedTotalBonus, result.TotalBonus);
            Assert.Equal(3, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "FAKE_BONUS_2" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "FAKE_BONUS_3" && b.Value == 3);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "FAKE_BONUS_1" && b.Value == 1);
        }

        [Fact]
        public void CalculateRescueTarget_WhenSourcesEmitNothing_ReturnsBaseDifficulty()
        {
            // Arrange
            var calculator = new RescueTargetCalculator();

            var sources = new[]
            {
                new FakeSource() // emits empty
            };
            RescueCalculationInput input = CreateInput();

            // Act
            var result = calculator.CalculateRescueTarget(
                difficultyNumber: 10,
                input: input,
                sources: sources);

            Assert.Equal(10, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
        }

        private static RescueCalculationInput CreateInput()
        {
            return new RescueCalculationInput(
                presentDisasterBonusKeys: [],
                RescueType.Space
            );
        }

        private sealed class FakeSource : IBonusModifierSource
        {
            private readonly IEnumerable<AppliedRescueModifier> _modifiers;

            public FakeSource(params int[] values)
            {
                _modifiers = values.Select(v => new AppliedRescueModifier
                {
                    Value = v,
                    SourceType = SourceType.DisasterCard,
                    Key = $"FAKE_BONUS_{v}"
                });
            }

            public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
            {
                return _modifiers;
            }
        }

    }
}
