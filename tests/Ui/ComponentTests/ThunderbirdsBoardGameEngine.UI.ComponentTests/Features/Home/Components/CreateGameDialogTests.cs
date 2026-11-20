using AngleSharp.Dom;
using Bunit;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.Home.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.Home.Components
{
    public class CreateGameDialogTests : BunitContext
    {
        public CreateGameDialogTests()
        {
            DialogJsInterop.Register(this);
        }

        [Fact]
        public void RendersCreateGameDialogCorrectly()
        {
            // Arrange
            var characters = CreateCharacters();

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters));

            // Assert
            Assert.Equal(2, cut.FindAll("select").Count);
            Assert.Contains("Create New Game", cut.Markup);
            Assert.Contains("Player 1 character", cut.Markup);
            Assert.Contains("Player 2 character", cut.Markup);
            Assert.Contains("Scott", cut.Markup);
            Assert.Contains("Virgil", cut.Markup);
            Assert.Contains("John", cut.Markup);
        }

        [Fact]
        public void RemovePlayerButtonDisabledWhenOnlyTwoPlayers()
        {
            // Arrange
            var characters = CreateCharacters();

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters));

            // Assert
            Assert.True(cut.Find("[data-testid='remove-player']").IsDisabled());
        }

        [Fact]
        public void AddPlayerIncreasesPlayerCount()
        {
            // Arrange
            var characters = CreateCharacters();

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters));

            var addPlayerButton = cut.Find("[data-testid='add-player']");
            addPlayerButton.Click();

            // Assert
            Assert.Contains("Player 3 character", cut.Markup);

            addPlayerButton.Click();
            Assert.Contains("Player 4 character", cut.Markup);
        }

        [Fact]
        public void AddPlayerButtonDisabledWhenFourPlayersAdded()
        {
            // Arrange
            var characters = CreateCharacters();

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters));

            // Add two more players to reach four players
            cut.Find("[data-testid='add-player']").Click();
            cut.Find("[data-testid='add-player']").Click();

            // Assert
            Assert.True(cut.Find("[data-testid='add-player']").IsDisabled());
        }

        [Fact]
        public void SelectedCharacterRemovedFromDropdownWhenSelected()
        {
            // Arrange
            var characters = CreateCharacters();

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters));

            var player1Dropdown = cut.Find("#player-character-0");
            player1Dropdown.Change("scott");

            // Assert
            var player2Dropdown = cut.Find("#player-character-1");
            Assert.Empty(player2Dropdown.QuerySelectorAll("option[value='scott']"));
        }

        [Fact]
        public void ConfirmButtonDisabledWhenNotAllPlayersSelected()
        {
            // Arrange
            var characters = CreateCharacters();

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters));

            var confirmButton = cut.Find("[data-testid='confirm-create']");

            cut.Find("#player-character-1").Change("scott");

            // Assert
            Assert.True(confirmButton.IsDisabled());
        }

        [Fact]
        public void ConfirmingGameCreationEmitsCharactersInTurnOrder()
        {
            // Arrange
            var characters = CreateCharacters();

            IReadOnlyList<string>? confirmedCharacters = null;

            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters)
                    .Add(
                        p => p.Confirmed,
                        selection => confirmedCharacters = selection));

            cut.Find("#player-character-0").Change("scott");
            cut.Find("#player-character-1").Change("virgil");

            // Act
            var confirm = cut.Find("[data-testid='confirm-create']");

            Assert.False(confirm.IsDisabled());

            confirm.Click();

            // Assert
            Assert.NotNull(confirmedCharacters);
            Assert.Equal(["scott", "virgil"], confirmedCharacters);
        }

        [Fact]
        public void ClickingCancelRaisesCancelEvent()
        {
            // Arrange
            bool cancelRaised = false;

            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, CreateCharacters())
                    .Add(p => p.Cancelled, () => cancelRaised = true));

            // Act
            cut.Find("[data-testid='cancel-create']").Click();

            // Assert
            Assert.True(cancelRaised);
        }

        [Fact]
        public void DisplaysErrorMessageWhenErrorOccurs()
        {
            // Arrange
            var characters = CreateCharacters();

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters)
                    .Add(p => p.ErrorMessage, "An error occurred"));

            // Assert
            Assert.Contains("An error occurred", cut.Markup);
        }

        [Fact]
        public async Task DialogCancellationDoesNotCancelWhileCreateIsInProgress()
        {
            // Arrange
            var characters = CreateCharacters();

            bool cancelRaised = false;

            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, characters)
                    .Add(p => p.IsCreating, true)
                    .Add(p => p.Cancelled, () => cancelRaised = true));

            // Act
            await cut.InvokeAsync(() => cut.Instance.HandleDialogCancelled());

            // Assert
            Assert.False(cancelRaised);
        }

        [Fact]
        public void RemovePlayerRemovesFinalPlayerSlot()
        {
            // Arrange
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, CreateCharacters()));

            // Act
            cut.Find("[data-testid='add-player']").Click();
            cut.Find("[data-testid='add-player']").Click();

            Assert.Equal(4, cut.FindAll("select").Count);

            cut.Find("[data-testid='remove-player']").Click();

            // Assert
            Assert.Equal(3, cut.FindAll("select").Count);
            Assert.DoesNotContain("Player 4 character", cut.Markup);
            Assert.False(cut.Find("[data-testid='add-player']").IsDisabled());
        }

        [Fact]
        public void CreationInProgressDisablesAllDialogActions()
        {
            // Arrange

            // Act
            var cut = Render<CreateGameDialog>(
                parameters => parameters
                    .Add(p => p.Characters, CreateCharacters())
                    .Add(p => p.IsCreating, true));

            // Assert
            Assert.True(cut.Find("[data-testid='add-player']").IsDisabled());
            Assert.True(cut.Find("[data-testid='remove-player']").IsDisabled());
            Assert.True(cut.Find("[data-testid='cancel-create']").IsDisabled());
            Assert.True(cut.Find("[data-testid='confirm-create']").IsDisabled());
            Assert.Contains(
                "Creating",
                cut.Find("[data-testid='confirm-create']").TextContent);
        }

        private static IReadOnlyList<SelectionOptionViewModel> CreateCharacters()
        {
            return
            [
                new("scott", "Scott"),
                new("virgil", "Virgil"),
                new("john", "John"),
                new("gordon", "Gordon"),
                new("alan", "Alan")
            ];
        }
    }
}
