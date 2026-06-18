using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class DisasterCardSelectorTests : BunitContext
    {
        [Fact]
        public void DisasterCardSelectorShouldRenderDisasterCards()
        {
            // Arrange
            var disasterCards = CreateDisasterCards();

            // Act
            var cut = Render<DisasterCardSelector>(parameters => parameters
                .Add(p => p.DisasterCards, disasterCards)
            );

            // Assert
            Assert.Contains("Fire", cut.Markup);
            Assert.Contains("Flood", cut.Markup);
        }

        [Fact]
        public void DisasterCardSelectorNotifiesParentWhenSelectionChanges()
        {
            // Arrange
            var disasterCards = CreateDisasterCards();

            string? selectedDisasterCardCode = "fire";

            var cut = Render<DisasterCardSelector>(parameters => parameters
                .Add(p => p.DisasterCards, disasterCards)
                .Add(
                    p => p.SelectedDisasterCardCodeChanged,
                    value => selectedDisasterCardCode = value));

            // Act            
            cut.Find("#disasterSelect").Change("flood");

            // Assert
            Assert.Equal("flood", selectedDisasterCardCode);
        }

        private static IReadOnlyList<DisasterCardViewModel> CreateDisasterCards()
        {
            return
            [
                new DisasterCardViewModel(
                    Code: "fire",
                    DisplayName: "Fire",
                    DifficultyNumber: 1,
                    RescueType: "Air",
                    Location: "Asia",
                    BonusConditions: [
                        new BonusConditionViewModel(Description: "Extra points for quick response", Key: "extra-points")
                    ],
                    Rewards: [
                        new RewardViewModel(Description: "Bonus points for quick response")
                    ]),
                new DisasterCardViewModel(
                    Code: "flood",
                    DisplayName: "Flood",
                    DifficultyNumber: 2,
                    RescueType: "Sea",
                    Location: "Europe",
                    BonusConditions: [
                        new BonusConditionViewModel(Description: "Extra points for quick response", Key: "extra-points")
                    ],
                    Rewards: [
                        new RewardViewModel(Description: "Bonus points for quick response")
                    ]),
            ];
        }
    }
}
