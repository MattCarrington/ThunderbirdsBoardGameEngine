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

        private static IReadOnlyList<DisasterCardSummaryViewModel> CreateDisasterCards()
        {
            return
            [
                new DisasterCardSummaryViewModel(
                    Code: "fire",
                    DisplayName: "Fire"),
                new DisasterCardSummaryViewModel(
                    Code: "flood",
                    DisplayName: "Flood")
            ];
        }
    }
}
