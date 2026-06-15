using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using ThunderbirdsBoardGameEngine.UI.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class BonusConditionListTests : BunitContext
    {
        [Fact]
        public void BonusConditionListShouldRenderCorrectly()
        {
            // Arrange
            var conditions = CreateSampleConditions();

            // Act
            var cut = Render<BonusConditionList>(parameters => parameters
                .Add(p => p.BonusConditions, conditions)
            );

            // Assert
            Assert.Contains("Condition 1", cut.Markup);
            Assert.Contains("Condition 2", cut.Markup);
            Assert.Contains("Condition 3", cut.Markup);
        }

        [Fact]
        public void BonusConditionShouldNotifyParentWhenBonusSelected()
        {
            // Arrange
            var conditions = CreateSampleConditions();

            BonusConditionChanged? receivedEvent = null;

            var cut = Render<BonusConditionList>(parameters => parameters
                .Add(p => p.BonusConditions, conditions)
                .Add(
                    p => p.BonusChanged,
                    value => receivedEvent = value));

            // Act
            cut.Find("[data-bonus-key='condition-1']")
                .Change(true);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("condition-1", receivedEvent!.Key);
            Assert.True(receivedEvent.Selected);
        }

        private static IReadOnlyList<BonusConditionViewModel> CreateSampleConditions()
        {
            return
            [
                new(Key: "condition-1", Description: "Condition 1"),
                new(Key: "condition-2", Description: "Condition 2"),
                new(Key: "condition-3", Description: "Condition 3")
            ];
        }
    }
}
