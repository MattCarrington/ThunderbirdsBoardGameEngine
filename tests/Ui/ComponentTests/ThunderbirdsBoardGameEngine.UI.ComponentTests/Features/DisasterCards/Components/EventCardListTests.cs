using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class EventCardListTests : BunitContext
    {
        [Fact]
        public void EventCardListShouldRenderCorrectly()
        {
            // Arrange
            var cards = CreateSampleCards();

            // Act
            var cut = Render<EventCardList>(parameters => parameters
                .Add(p => p.EventCards, cards)
            );

            // Assert
            Assert.Contains("Event Card 1", cut.Markup);
            Assert.Contains("Event Card 2", cut.Markup);
            Assert.Contains("Event Card 3", cut.Markup);
        }

        [Fact]
        public void EventCardShouldNotifyParentWhenCardSelected()
        {
            // Arrange
            var cards = CreateSampleCards();

            EventCardChanged? receivedEvent = null;

            var cut = Render<EventCardList>(parameters => parameters
                .Add(p => p.EventCards, cards)
                .Add(
                    p => p.EventCardChanged,
                    value => receivedEvent = value));

            // Act
            cut.Find("[data-event-key='event-card-1']")
                .Change(true);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("event-card-1", receivedEvent!.Key);
            Assert.True(receivedEvent.Selected);
        }

        [Fact]
        public void EventCardShouldNotifyParentWhenCardDeselected()
        {
            // Arrange
            var cards = CreateSampleCards();

            EventCardChanged? receivedEvent = null;

            var cut = Render<EventCardList>(parameters => parameters
                .Add(p => p.EventCards, cards)
                .Add(
                    p => p.EventCardChanged,
                    value => receivedEvent = value));

            // Act
            cut.Find("[data-event-key='event-card-2']")
                .Change(true);

            cut.Find("[data-event-key='event-card-2']")
                .Change(false);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("event-card-2", receivedEvent!.Key);
            Assert.False(receivedEvent.Selected);
        }

        private static IReadOnlyList<CardModifierViewModel> CreateSampleCards()
        {
            return
            [
                new(Key: "event-card-1", DisplayName: "Event Card 1"),
                new(Key: "event-card-2", DisplayName: "Event Card 2"),
                new(Key: "event-card-3", DisplayName: "Event Card 3")
            ];
        }
    }
}
