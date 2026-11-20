using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Home;
using ThunderbirdsBoardGameEngine.UI.Features.Home.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.Home
{
    public class HomePageTests : BunitContext
    {
        [Fact]
        public void ResumeGameShouldNavigateWhenIdentifierIsValid()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var context = new HomePageTestContext(this);

            var navigation = Services.GetRequiredService<NavigationManager>();

            // Act
            var cut = Render<HomePage>();

            cut.Find("#game-identifier").Change(gameId.ToString());
            cut.Find("[data-testid='resume-game']").Click();

            // Assert
            Assert.EndsWith($"/games/{gameId:D}", navigation.Uri);
        }

        [Theory]
        [InlineData("invalid-guid")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void ResumeGameDisplaysErrorWhenIdentifierIsInvalid(string identifier)
        {
            // Arrange
            var context = new HomePageTestContext(this);

            // Act
            var cut = Render<HomePage>();

            cut.Find("#game-identifier").Change(identifier);

            cut.Find("[data-testid='resume-game']").Click();

            // Assert
            Assert.Contains("Enter a valid game identifier.", cut.Markup);
        }

        [Fact]
        public async Task ClickingCreateShouldOpenCreateGameDialogAsync()
        {
            // Arrange
            var characters = CreateCharacters();

            var context = new HomePageTestContext(this);
            context.CharacterService.GetAll().Returns(characters);

            DialogJsInterop.Register(this);

            // Act
            var cut = Render<HomePage>();
            cut.Find("[data-testid='create-game']").Click();

            // Assert
            var dialog = Assert.Single(cut.FindComponents<CreateGameDialog>());

            Assert.Equal(characters, dialog.Instance.Characters);

            context.CharacterService.Received(1).GetAll();

            await context.GameService.DidNotReceiveWithAnyArgs()
                .CreateGameAsync(default!);
        }

        [Fact]
        public async Task CancellingCreateGameClosesDialogWithoutCreatingGame()
        {
            // Arrange
            var characters = CreateCharacters();

            var context = new HomePageTestContext(this);
            context.CharacterService.GetAll().Returns(characters);

            DialogJsInterop.Register(this);

            // Act
            var cut = Render<HomePage>();

            cut.Find("[data-testid='create-game']").Click();

            var dialog = Assert.Single(cut.FindComponents<CreateGameDialog>());

            dialog.Find("#player-character-0").Change("gordon");
            dialog.Find("#player-character-1").Change("scott");

            dialog.Find("[data-testid='cancel-create']").Click();

            // Assert
            Assert.Empty(cut.FindComponents<CreateGameDialog>());

            await context.GameService.DidNotReceiveWithAnyArgs()
                .CreateGameAsync(default!);
        }

        [Fact]
        public void ConfirmCreateGameDialogShouldCreateGameWithSelectedCharacters()
        {
            // Arrange
            var characters = CreateCharacters();

            IReadOnlyList<string>? confirmedCharacters = null;

            var context = new HomePageTestContext(this);
            context.CharacterService.GetAll().Returns(characters);
            context.GameService.CreateGameAsync(Arg.Do<IReadOnlyList<string>>(x => confirmedCharacters = x))
                .Returns(Guid.NewGuid());
            DialogJsInterop.Register(this);
            // Act
            var cut = Render<HomePage>();
            cut.Find("[data-testid='create-game']").Click();

            var dialog = Assert.Single(cut.FindComponents<CreateGameDialog>());

            dialog.Find("#player-character-0").Change("john");
            dialog.Find("#player-character-1").Change("gordon");

            dialog.Find("[data-testid='confirm-create']").Click();

            // Assert
            var expectedCharacters = new List<string> { "john", "gordon" };

            Assert.NotNull(confirmedCharacters);
            Assert.Equal(expectedCharacters, confirmedCharacters);

            context.GameService.Received(1).CreateGameAsync(Arg.Is<IReadOnlyList<string>>(x => x.SequenceEqual(expectedCharacters)));
        }

        [Fact]
        public void SuccessfulGameCreationNavigatesToGameDashboard()
        {
            // Arrange
            var characters = CreateCharacters();

            var gameId = Guid.NewGuid();

            var context = new HomePageTestContext(this);
            context.CharacterService.GetAll().Returns(characters);
            context.GameService.CreateGameAsync(Arg.Any<IReadOnlyList<string>>()).Returns(gameId);

            var navigation = Services.GetRequiredService<NavigationManager>();

            DialogJsInterop.Register(this);

            // Act
            var cut = Render<HomePage>();
            cut.Find("[data-testid='create-game']").Click();

            var dialog = Assert.Single(cut.FindComponents<CreateGameDialog>());
            dialog.Find("#player-character-0").Change("alan");
            dialog.Find("#player-character-1").Change("virgil");
            dialog.Find("[data-testid='confirm-create']").Click();

            // Assert
            Assert.EndsWith($"/games/{gameId:D}", navigation.Uri);
        }

        [Fact]
        public void DialogShouldNotCloseAndDisplayErrorWhenGameCreationFails()
        {
            // Arrange
            var characters = CreateCharacters();

            var context = new HomePageTestContext(this);
            context.CharacterService.GetAll().Returns(characters);
            context.GameService.CreateGameAsync(Arg.Any<IReadOnlyList<string>>())
                .ThrowsAsync(new Exception("Test exception"));

            DialogJsInterop.Register(this);
            // Act

            var cut = Render<HomePage>();
            cut.Find("[data-testid='create-game']").Click();

            var dialog = Assert.Single(cut.FindComponents<CreateGameDialog>());
            dialog.Find("#player-character-0").Change("alan");
            dialog.Find("#player-character-1").Change("virgil");
            dialog.Find("[data-testid='confirm-create']").Click();

            // Assert
            var updatedDialog = Assert.Single(cut.FindComponents<CreateGameDialog>());

            Assert.Equal("The game could not be created. Please try again.", updatedDialog.Instance.ErrorMessage);

            Assert.Contains("The game could not be created. Please try again.", updatedDialog.Markup);
        }

        [Fact]
        public async Task PendingCreationPassesIsCreatingToDialog()
        {
            // Arrange
            var characters = CreateCharacters();

            var pendingCreation = new TaskCompletionSource<Guid>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            var context = new HomePageTestContext(this);
            context.CharacterService.GetAll().Returns(characters);
            context.GameService.CreateGameAsync(Arg.Any<IReadOnlyList<string>>()).Returns(pendingCreation.Task);

            DialogJsInterop.Register(this);

            var cut = Render<HomePage>();

            cut.Find("[data-testid='create-game']").Click();

            var dialog = cut.FindComponent<CreateGameDialog>();

            dialog.Find("#player-character-0").Change("alan");
            dialog.Find("#player-character-1").Change("virgil");

            // Act
            var confirmationTask = dialog
                .Find("[data-testid='confirm-create']")
                .ClickAsync(new MouseEventArgs());

            // Assert while the service call remains incomplete
            cut.WaitForAssertion(() =>
            {
                var updatedDialog =
                    cut.FindComponent<CreateGameDialog>();

                Assert.True(updatedDialog.Instance.IsCreating);
            });

            // Complete the pending operation so the event finishes
            pendingCreation.SetResult(Guid.NewGuid());

            await confirmationTask;
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
