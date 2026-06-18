using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class DisasterCardDetailTests : BunitContext
    {
        [Fact]
        public void DisasterCardDetailsShouldRender()
        {
            // Arrange
            var disasterCard = new DisasterCardViewModel(
                Code: "fire",
                DisplayName: "Fire",
                DifficultyNumber: 1,
                RescueType: "Air",
                Location: "Asia",
                BonusConditions: [
                    new BonusConditionViewModel(Description: "Extra points for quick response", Key: "extra-points")
                ],
                Rewards: [
                    new RewardViewModel(Description: "Reward 1"),
                    new RewardViewModel(Description: "Reward 2")
                ]
            );

            // Act
            var cut = Render<DisasterCardDetails>(parameters => parameters.Add(p => p.Card, disasterCard));

            // Assert
            Assert.Contains("Fire", cut.Markup);
            Assert.Contains("Difficulty: 1", cut.Markup);
            Assert.Contains("Rescue Type: Air", cut.Markup);
            Assert.Contains("Location: Asia", cut.Markup);
        }
    }
}
