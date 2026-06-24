using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class FabCardListTests : BunitContext
    {
        [Fact]
        public void FabCardListShouldRenderCorrectly()
        {
            // Arrange
            var cards = CreateSampleCards();

            // Act
            var cut = Render<FabCardList>(parameters => parameters
                .Add(p => p.FabCards, cards)
            );

            // Assert
            Assert.Contains("F.A.B. Card 1", cut.Markup);
            Assert.Contains("F.A.B. Card 2", cut.Markup);
            Assert.Contains("F.A.B. Card 3", cut.Markup);
        }

        [Fact]
        public void FabCardShouldNotifyParentWhenCardSelected()
        {
            // Arrange
            var cards = CreateSampleCards();

            FabCardChanged? receivedEvent = null;

            var cut = Render<FabCardList>(parameters => parameters
                .Add(p => p.FabCards, cards)
                .Add(
                    p => p.FabCardChanged,
                    value => receivedEvent = value));

            // Act
            cut.Find("[data-fab-key='fab-card-1']")
                .Change(true);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("fab-card-1", receivedEvent!.Key);
            Assert.True(receivedEvent.Selected);
        }

        [Fact]
        public void FabCardShouldNotifyParentWhenCardDeselected()
        {
            // Arrange
            var cards = CreateSampleCards();

            FabCardChanged? receivedEvent = null;

            var cut = Render<FabCardList>(parameters => parameters
                .Add(p => p.FabCards, cards)
                .Add(
                    p => p.FabCardChanged,
                    value => receivedEvent = value));

            // Act
            cut.Find("[data-fab-key='fab-card-2']")
                .Change(true);

            cut.Find("[data-fab-key='fab-card-2']")
                .Change(false);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("fab-card-2", receivedEvent!.Key);
            Assert.False(receivedEvent.Selected);
        }

        private static IReadOnlyList<CardModifierViewModel> CreateSampleCards()
        {
            return
            [
                new(Key: "fab-card-1", DisplayName: "F.A.B. Card 1"),
                new(Key: "fab-card-2", DisplayName: "F.A.B. Card 2"),
                new(Key: "fab-card-3", DisplayName: "F.A.B. Card 3")
            ];
        }
    }
}
