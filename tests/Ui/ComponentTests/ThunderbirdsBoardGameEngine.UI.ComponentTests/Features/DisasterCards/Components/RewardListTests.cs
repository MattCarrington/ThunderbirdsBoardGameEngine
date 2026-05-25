using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class RewardListTests : BunitContext
    {
        [Fact]
        public void RewardListShouldRenderCorrectly()
        {
            // Arrange
            var rewards = CreateSampleRewards();

            // Act
            var cut = Render<RewardList>(parameters => parameters
                .Add(p => p.Rewards, rewards)
            );

            // Assert
            Assert.Contains("Reward 1", cut.Markup);
            Assert.Contains("Reward 2", cut.Markup);
            Assert.Contains("Reward 3", cut.Markup);
        }

        private static IReadOnlyList<RewardViewModel> CreateSampleRewards()
        {
            return
            [
                new(Description: "Reward 1"),
                new(Description: "Reward 2"),
                new(Description: "Reward 3")
            ];
        }
    }
}
