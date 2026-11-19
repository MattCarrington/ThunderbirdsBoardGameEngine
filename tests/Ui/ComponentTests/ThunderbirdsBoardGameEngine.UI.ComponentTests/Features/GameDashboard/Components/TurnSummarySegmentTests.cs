using Bunit;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.GameDashboard.Components
{
    public class TurnSummarySegmentTests : BunitContext
    {
        [Fact]
        public void DisplaysActiveCharacter()
        {
            // Arrange
            var summary = CreateSummary();

            // Act
            var cut = Render<TurnSummarySegment>(
                parameters => parameters
                    .Add(component => component.Summary, summary));

            // Assert
            Assert.Contains(
                summary.ActiveCharacter.CharacterDisplayName,
                cut.Markup);
        }

        [Fact]
        public void DisplaysCharactersInTurnOrder()
        {
            // Arrange
            var summary = CreateSummary();

            // Act
            var cut = Render<TurnSummarySegment>(
                parameters => parameters
                    .Add(component => component.Summary, summary));

            // Assert
            var displayedCharacters = cut.FindAll("[data-testid='turn-order-character']")
                .Select(element => element.TextContent.Trim())
                .ToArray();

            Assert.Equal(
                summary.TurnOrder
                    .Select(character => character.CharacterDisplayName),
                displayedCharacters);
        }

        [Fact]
        public void HighlightsActiveCharacterInTurnOrder()
        {
            // Arrange
            var summary = CreateSummary();

            var cut = Render<TurnSummarySegment>(
                parameters => parameters
                    .Add(component => component.Summary, summary));

            // Act
            var active = cut.Find($"[data-character-code='{summary.ActiveCharacter.CharacterCode}']");

            // Assert
            Assert.Contains("fw-bold", active.ClassList);
            Assert.Contains("text-primary", active.ClassList);
        }

        private static TurnSummaryDashboardViewModel CreateSummary()
        {
            var characterOne = GameDashboardViewModelFactory.CreateTurnOrderCharacter(
                characterCode: "scott",
                characterDisplayName: "Scott");

            var characterTwo = GameDashboardViewModelFactory.CreateTurnOrderCharacter(
                characterCode: "virgil",
                characterDisplayName: "Virgil");

            var characterThree = GameDashboardViewModelFactory.CreateTurnOrderCharacter(
                characterCode: "alan",
                characterDisplayName: "Alan");

            return new TurnSummaryDashboardViewModel(
                ActiveCharacter: characterTwo,
                TurnOrder: [characterOne, characterTwo, characterThree]);
        }
    }
}
