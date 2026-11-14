using Bunit;
using Microsoft.AspNetCore.Components.Web;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.GameDashboard
{
    public class GameDashboardTests : BunitContext
    {
        [Fact]
        public void CallsGameServiceOnInitialisation()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var response = GameDashboardViewModelFactory.CreateDashboard(gameId);

            var context = new GameDashboardPageTestContext(this);
            context.GameService.GetGameAsync(Arg.Any<Guid>()).Returns(response);

            // Act
            _ = Render<GameDashboardPage>(
                parameters => parameters
                    .Add(page => page.GameId, gameId));

            // Assert
            context.GameService.Received(1).GetGameAsync(gameId);
        }

        [Fact]
        public void CanSuccessfullyRetrieveGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var response = GameDashboardViewModelFactory.CreateDashboard(gameId);

            var context = new GameDashboardPageTestContext(this);
            context.GameService.GetGameAsync(gameId).Returns(response);

            // Act
            var cut = Render<GameDashboardPage>(
                parameters => parameters
                    .Add(page => page.GameId, gameId));
            // Assert
            Assert.Contains("Thunderbird Machines", cut.Markup);
        }

        [Fact]
        public void DisplaysGameCouldNotBeFoundIfGameReturnsNull()
        {
            // Arrange
            var context = new GameDashboardPageTestContext(this);
            context.GameService.GetGameAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var cut = Render<GameDashboardPage>(
                parameters => parameters
                    .Add(page => page.GameId, Guid.NewGuid()));

            // Assert
            Assert.Contains("This game could not be found.", cut.Markup);
        }

        [Fact]
        public void DisplaysGameCouldNotBeLoadedIfGameReturnsError()
        {
            // Arrange
            var context = new GameDashboardPageTestContext(this);
            context.GameService.GetGameAsync(Arg.Any<Guid>()).Throws(new HttpRequestException());

            // Act
            var cut = Render<GameDashboardPage>(
                parameters => parameters
                    .Add(page => page.GameId, Guid.NewGuid()));

            // Assert
            Assert.Contains("The game could not be loaded. Please try again.", cut.Markup);
        }

        [Fact]
        public void DisplaysGameUponRetryWhenSecondRequestSucceeds()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var response = GameDashboardViewModelFactory.CreateDashboard(gameId);
            var attempts = 0;

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(gameId)
                .Returns(_ =>
                {
                    attempts++;

                    if (attempts == 1)
                    {
                        throw new HttpRequestException();
                    }

                    return response;
                });

            var cut = Render<GameDashboardPage>(
                parameters => parameters
                    .Add(page => page.GameId, gameId));

            Assert.Contains(
                "The game could not be loaded.",
                cut.Markup);

            // Act
            cut.Find("[data-testid='retry-game']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("Thunderbird Machines", cut.Markup);
                Assert.DoesNotContain(
                    "The game could not be loaded.",
                    cut.Markup);
            });

            context.GameService
                .Received(2)
                .GetGameAsync(gameId);
        }

        [Fact]
        public void DisplaysReturnHomeLinkWhenGameNotFound()
        {
            // Arrange
            var context = new GameDashboardPageTestContext(this);
            context.GameService.GetGameAsync(Arg.Any<Guid>()).Returns((GameDashboardViewModel?)null);

            // Act
            var cut = Render<GameDashboardPage>();

            // Assert
            var link = cut.Find("a[href='/']");

            Assert.Equal("Return home", link.TextContent.Trim());
        }

        [Fact]
        public void ClickingMoveShouldOpenDialogForSelectedMachine()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = GameDashboardViewModelFactory.CreateDashboard(gameId);

            var context = new GameDashboardPageTestContext(this);
            context.GameService.GetGameAsync(gameId).Returns(game);

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act
            cut.WaitForElement("[data-testid='move-thunderbird-1']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                var dialog = cut.FindComponent<MoveThunderbirdDialog>();

                Assert.Equal("thunderbird-1", dialog.Instance.Machine.ThunderbirdCode);
            });
        }

        [Fact]
        public async Task SuccessfulMoveShouldUpdateDashboardAndCloseDialog()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var originalMachine = GameDashboardViewModelFactory.CreateMachine(
                locationCode: "south-pacific",
                locationDisplayName: "South Pacific");

            var movedMachine = GameDashboardViewModelFactory.CreateMachine(
                locationCode: "europe",
                locationDisplayName: "Europe");

            var originalGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: gameId,
                machines: [originalMachine]);

            var movedGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: gameId,
                machines: [movedMachine]);

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(gameId)
                .Returns(originalGame);

            context.MovementClientService
                .GetAccessibleLocationsAsync("thunderbird-1")
                .Returns(
                [
                    new("south-pacific", "South Pacific"),
                    new("europe", "Europe")
                ]);

            context.GameService
                .MoveThunderbirdAsync(
                    gameId,
                    "thunderbird-1",
                    "europe")
                .Returns(new ThunderbirdMovementResult(
                    ThunderbirdMovementOutcome.Success,
                    movedGame));

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act
            cut.WaitForElement("[data-testid='move-thunderbird-1']")
                .Click();

            var dialog = cut.FindComponent<MoveThunderbirdDialog>();

            dialog.Find("#move-destination").Change("europe");

            await dialog
                .Find("[data-testid='confirm-move']")
                .ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("Europe", cut.Markup);
                Assert.Empty(cut.FindComponents<MoveThunderbirdDialog>());
            });

            await context.GameService.Received(1)
                .GetGameAsync(gameId);

            await context.GameService.Received(1)
                .MoveThunderbirdAsync(
                    gameId,
                    "thunderbird-1",
                    "europe");
        }

        [Theory]
        [InlineData(ThunderbirdMovementOutcome.Rejected, "That move is not permitted.")]
        [InlineData(ThunderbirdMovementOutcome.NotFound, "The game or Thunderbird Machine could not be found.")]

        public async Task UnsuccessfulMoveShouldKeepDialogOpenAndDisplayError(ThunderbirdMovementOutcome outcome, string expectedMessage)
        {
            var gameId = Guid.NewGuid();

            var originalMachine = GameDashboardViewModelFactory.CreateMachine(
                locationCode: "south-pacific",
                locationDisplayName: "South Pacific");

            var movedMachine = GameDashboardViewModelFactory.CreateMachine(
                locationCode: "europe",
                locationDisplayName: "Europe");

            var originalGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: gameId,
                machines: [originalMachine]);

            var movedGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: gameId,
                machines: [movedMachine]);

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(gameId)
                .Returns(originalGame);

            context.MovementClientService
                .GetAccessibleLocationsAsync("thunderbird-1")
                .Returns(
                [
                    new("south-pacific", "South Pacific"),
                    new("europe", "Europe")
                ]);

            context.GameService
                .MoveThunderbirdAsync(gameId, "thunderbird-1", "europe")
                .Returns(new ThunderbirdMovementResult(outcome, null));

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act
            cut.WaitForElement("[data-testid='move-thunderbird-1']")
                .Click();

            var dialog = cut.FindComponent<MoveThunderbirdDialog>();

            dialog.Find("#move-destination").Change("europe");

            await dialog
                .Find("[data-testid='confirm-move']")
                .ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Single(cut.FindComponents<MoveThunderbirdDialog>());
                Assert.Contains(expectedMessage, cut.Find("[role='alert']").TextContent);
            });
        }

        [Fact]
        public async Task CancellingMoveShouldNotUpdateDashboardButCloseDialogAsync()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = GameDashboardViewModelFactory.CreateDashboard(gameId: gameId);

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(gameId)
                .Returns(game);

            context.MovementClientService
                .GetAccessibleLocationsAsync("thunderbird-1")
                .Returns(
                [
                    new("south-pacific", "South Pacific"),
                    new("europe", "Europe")
                ]);

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act
            cut.WaitForElement("[data-testid='move-thunderbird-1']")
                .Click();

            var dialog = cut.FindComponent<MoveThunderbirdDialog>();

            dialog.Find("#move-destination").Change("europe");

            await dialog
                .Find("[data-testid='cancel-move']")
                .ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("South Pacific", cut.Markup);
                Assert.Empty(cut.FindComponents<MoveThunderbirdDialog>());
            });

            await context.GameService.Received(1)
                .GetGameAsync(gameId);

            await context.GameService.DidNotReceiveWithAnyArgs()
                .MoveThunderbirdAsync(
                    default,
                    default!,
                    default!);
        }

        [Fact]
        public async Task DialogDisplaysWarningWhenNoAccessibleLocations()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = GameDashboardViewModelFactory.CreateDashboard();

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(gameId)
                .Returns(game);

            context.MovementClientService
                .GetAccessibleLocationsAsync("thunderbird-1")
                .ThrowsAsync(new HttpRequestException());

            DialogJsInterop.Register(this);

            // Act
            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            cut.WaitForElement("[data-testid='move-thunderbird-1']")
                .Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Single(cut.FindComponents<MoveThunderbirdDialog>());

                Assert.Contains(
                    "Movement destinations could not be loaded.",
                    cut.Find("[role='alert']").TextContent);
            });

            await context.MovementClientService.Received(1)
                .GetAccessibleLocationsAsync("thunderbird-1");

            await context.GameService.DidNotReceiveWithAnyArgs()
                .MoveThunderbirdAsync(default, default!, default!);
        }

        [Fact]
        public async Task CancellingDialogWhileMovingDoesNotRaiseCancelled()
        {
            // Arrange
            var cancelled = false;

            DialogJsInterop.Register(this);

            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(
                        component => component.Machine,
                        GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        [new("venus", "Venus")])
                    .Add(component => component.IsMoving, true)
                    .Add(
                        component => component.Cancelled,
                        () => cancelled = true));

            // Act
            await cut.Instance.HandleDialogCancelled();

            // Assert
            Assert.False(cancelled);
        }

        [Fact]
        public async Task CancellingDialogWhenIdleRaisesCancelled()
        {
            // Arrange
            var cancelled = false;

            DialogJsInterop.Register(this);

            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(
                        component => component.Machine,
                        GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        [new("venus", "Venus")])
                    .Add(
                        component => component.Cancelled,
                        () => cancelled = true));

            // Act
            await cut.Instance.HandleDialogCancelled();

            // Assert
            Assert.True(cancelled);
        }

        [Fact]
        public async Task CancellingTransferShouldNotUpdateDashboardButCloseDialogAsync()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var occupants = new List<OccupantsDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateOccupant()
            };

            var thunderbirds = new List<ThunderbirdMachinesDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-1", occupants: occupants),
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-2")
            };

            var game = GameDashboardViewModelFactory.CreateDashboard(gameId: gameId, machines: thunderbirds);

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(gameId)
                .Returns(game);

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act
            cut.WaitForElement("[data-testid='transfer-thunderbird-1']")
                .Click();

            var dialog = cut.FindComponent<TransferCharactersDialog>();

            Assert.Contains("Transfer from", dialog.Markup);

            await dialog
                .Find("[data-testid='cancel-transfer']")
                .ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("South Pacific", cut.Markup);
                Assert.Empty(cut.FindComponents<TransferCharactersDialog>());
            });

            await context.GameService.Received(1)
                .GetGameAsync(gameId);

            await context.GameService.DidNotReceiveWithAnyArgs()
                .TransferCharactersAsync(
                    default,
                    default!);
        }

        [Fact]
        public async Task SuccessfulSingleTransferShouldUpdateDashboardAndCloseDialogAsync()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var occupants = new List<OccupantsDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateOccupant()
            };

            var originalThunderbirds = new List<ThunderbirdMachinesDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-1", occupants: occupants),
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-2")
            };

            var updatedThunderbirds = new List<ThunderbirdMachinesDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-1"),
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-2", occupants: occupants)
            };

            var game = GameDashboardViewModelFactory.CreateDashboard(gameId: gameId, machines: originalThunderbirds);

            var context = new GameDashboardPageTestContext(this);
            context.GameService
                .GetGameAsync(gameId)
                .Returns(game);

            context.GameService
                .TransferCharactersAsync(
                    gameId,
                    new CharacterTransferSelection(
                        SourceThunderbirdCode: "thunderbird-1",
                        CharacterCode: "scott",
                        DestinationThunderbirdCode: "thunderbird-2",
                        ReturningCharacterCode: null)
                    )
                .Returns(new CharacterTransferResult(
                    CharacterTransferOutcome.Success,
                    GameDashboardViewModelFactory.CreateDashboard(gameId: gameId, machines: updatedThunderbirds)));

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act
            cut.WaitForElement("[data-testid='transfer-thunderbird-1']")
                .Click();

            var dialog = cut.FindComponent<TransferCharactersDialog>();

            await dialog
                .Find("[data-testid='confirm-transfer']")
                .ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Empty(cut.FindComponents<TransferCharactersDialog>());

                var sourceRow = cut.Find("[data-testid='machine-thunderbird-1']");

                var destinationRow = cut.Find("[data-testid='machine-thunderbird-2']");

                Assert.Contains("Empty", sourceRow.TextContent);
                Assert.Contains("Scott", destinationRow.TextContent);
            });

            await context.GameService.Received(1)
                .GetGameAsync(gameId);

            await context.GameService.Received(1)
                .TransferCharactersAsync(
                    gameId,
                    new CharacterTransferSelection(
                        SourceThunderbirdCode: "thunderbird-1",
                        CharacterCode: "scott",
                        DestinationThunderbirdCode: "thunderbird-2",
                        ReturningCharacterCode: null));
        }

        [Theory]
        [InlineData(CharacterTransferOutcome.Rejected, "That character transfer is not permitted.")]
        [InlineData(CharacterTransferOutcome.NotFound, "The game, character, or Thunderbird Machine could not be found.")]

        public async Task UnsuccessfulTransferShouldKeepDialogOpenAndDisplayError(CharacterTransferOutcome outcome, string expectedMessage)
        {
            var gameId = Guid.NewGuid();

            var occupants = new List<OccupantsDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateOccupant()
            };

            var originalThunderbirds = new List<ThunderbirdMachinesDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-1", occupants: occupants),
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-2")
            };

            var dashboard = GameDashboardViewModelFactory.CreateDashboard(gameId: gameId, machines: originalThunderbirds);

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(gameId)
                .Returns(dashboard);

            context.GameService
                .TransferCharactersAsync(
                    gameId,
                    new CharacterTransferSelection(
                        SourceThunderbirdCode: "thunderbird-1",
                        CharacterCode: "scott",
                        DestinationThunderbirdCode: "thunderbird-2",
                        ReturningCharacterCode: null)
                    )
                .Returns(new CharacterTransferResult(outcome, null));

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act
            cut.WaitForElement("[data-testid='transfer-thunderbird-1']")
                .Click();

            var dialog = cut.FindComponent<TransferCharactersDialog>();

            await dialog
                .Find("[data-testid='confirm-transfer']")
                .ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Single(cut.FindComponents<TransferCharactersDialog>());
                Assert.Contains(expectedMessage, cut.Find("[role='alert']").TextContent);

                var sourceRow = cut.Find("[data-testid='machine-thunderbird-1']");

                var destinationRow = cut.Find("[data-testid='machine-thunderbird-2']");

                Assert.Contains("Scott", sourceRow.TextContent);
                Assert.Contains("Empty", destinationRow.TextContent);
            });
        }

        [Fact]
        public async Task ChangingGameIdWhileTheTransferDialogIsOpenLoadsTheNewGameAndClosesTheDialog()
        {
            // Arrange
            var gameId1 = Guid.NewGuid();
            var gameId2 = Guid.NewGuid();

            var occupants = new List<OccupantsDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateOccupant()
            };

            var originalThunderbirds = new List<ThunderbirdMachinesDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-1", occupants: occupants),
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-2")
            };

            var dashboard1 = GameDashboardViewModelFactory.CreateDashboard(gameId: gameId1, machines: originalThunderbirds);
            var dashboard2 = GameDashboardViewModelFactory.CreateDashboard(gameId: gameId2, machines: originalThunderbirds);

            var context = new GameDashboardPageTestContext(this);
            context.GameService
                .GetGameAsync(gameId1)
                .Returns(dashboard1);
            context.GameService
                .GetGameAsync(gameId2)
                .Returns(dashboard2);

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId1));

            // Act
            cut.WaitForElement("[data-testid='transfer-thunderbird-1']")
                .Click();

            cut.Render(parameters => parameters
                .Add(p => p.GameId, gameId2));

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Empty(cut.FindComponents<TransferCharactersDialog>());

                var sourceRow = cut.Find("[data-testid='machine-thunderbird-1']");

                Assert.Contains("Scott", sourceRow.TextContent);
            });
        }

        [Fact]
        public async Task ObsoleteTransferResultDoesNotReplaceNewGameOrCloseItsDialog()
        {
            // Arrange
            var oldGameId = Guid.NewGuid();
            var newGameId = Guid.NewGuid();

            var oldGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: oldGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-1",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "scott", characterDisplayName: "Scott")
                        ]),
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-2")
                ]);

            var newGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: oldGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-3",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "alan", characterDisplayName: "Alan"),
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "virgil", characterDisplayName: "Virgil"),
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "gordon", characterDisplayName: "Gordon")
                        ]),
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-4")
                ]);

            var oldTransferredGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: oldGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-1"),
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-2",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "scott")
                        ])
                ]);

            var oldTransferCompletion =
                new TaskCompletionSource<CharacterTransferResult>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            var context = new GameDashboardPageTestContext(this);

            context.GameService
                .GetGameAsync(oldGameId)
                .Returns(oldGame);

            context.GameService
                .GetGameAsync(newGameId)
                .Returns(newGame);

            context.GameService
                .TransferCharactersAsync(
                    oldGameId,
                    Arg.Any<CharacterTransferSelection>())
                .Returns(oldTransferCompletion.Task);

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(page => page.GameId, oldGameId));

            cut.WaitForElement("[data-testid='transfer-thunderbird-1']")
                .Click();

            var oldDialog = cut.FindComponent<TransferCharactersDialog>();

            // Start, but deliberately do not await, the old transfer.
            var obsoleteTransferTask = oldDialog
                .Find("[data-testid='confirm-transfer']")
                .ClickAsync(new MouseEventArgs());

            // Ensure the old operation has reached the service boundary.
            await context.GameService.Received(1)
                .TransferCharactersAsync(
                    oldGameId,
                    Arg.Any<CharacterTransferSelection>());

            // Load another game into the same component instance.
            cut.Render(parameters => parameters
                .Add(page => page.GameId, newGameId));

            cut.WaitForAssertion(() =>
            {
                var row = cut.Find("[data-testid='machine-thunderbird-3']");

                Assert.Contains("Virgil", row.TextContent);
                Assert.DoesNotContain("Scott", row.TextContent);
            });

            // Open a dialog belonging to the new game. The obsolete result must not close this dialog.
            cut.Find("[data-testid='transfer-thunderbird-3']")
                .Click();

            Assert.Single(cut.FindComponents<TransferCharactersDialog>());

            // Allow the previous game's request to complete.
            oldTransferCompletion.SetResult(
                new CharacterTransferResult(
                    CharacterTransferOutcome.Success,
                    oldTransferredGame));

            await obsoleteTransferTask;

            // Assert
            cut.WaitForAssertion(() =>
            {
                var row = cut.Find("[data-testid='machine-thunderbird-3']");

                Assert.Contains("Virgil", row.TextContent);
                Assert.DoesNotContain("Scott", row.TextContent);

                Assert.Single(cut.FindComponents<TransferCharactersDialog>());
            });
        }

        [Fact]
        public async Task ObsoleteMovementResultDoesNotReplaceNewGame()
        {
            // Arrange
            var oldGameId = Guid.NewGuid();
            var newGameId = Guid.NewGuid();

            var oldGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: oldGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-1",
                        locationCode: "south-pacific",
                        locationDisplayName: "South Pacific",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "scott", characterDisplayName: "Scott")
                        ])
                ]);

            var newGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: newGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-2",
                        locationCode: "europe",
                        locationDisplayName: "Europe",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "virgil", characterDisplayName: "Virgil")
                        ])
                ]);

            var oldMovedGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: oldGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-1",
                        locationCode: "australia",
                        locationDisplayName: "Australia",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "scott", characterDisplayName: "Scott")
                        ])
                ]);

            var oldMoveCompletion =
                new TaskCompletionSource<ThunderbirdMovementResult>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            var context = new GameDashboardPageTestContext(this);
            context.GameService
                .GetGameAsync(oldGameId)
                .Returns(oldGame);
            context.GameService
                .GetGameAsync(newGameId)
                .Returns(newGame);
            context.GameService
                .MoveThunderbirdAsync(
                    oldGameId,
                    Arg.Any<string>(),
                    Arg.Any<string>())
                .Returns(oldMoveCompletion.Task);

            context.MovementClientService
                .GetAccessibleLocationsAsync("thunderbird-1")
                .Returns(
                [
                    new("south-pacific", "South Pacific"),
                    new("europe", "Europe"),
                    new("australia", "Australia")
                ]);

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(page => page.GameId, oldGameId));

            cut.WaitForElement("[data-testid='move-thunderbird-1']")
                .Click();

            var dialog = cut.FindComponent<MoveThunderbirdDialog>();
            dialog.Find("#move-destination").Change("europe");

            // Start, but deliberately do not await, the old move.
            var obsoleteMoveTask = dialog.Find("[data-testid='confirm-move']")
                .ClickAsync(new MouseEventArgs());

            // Ensure the old operation has reached the service boundary.
            await context.GameService.Received(1)
                .MoveThunderbirdAsync(
                    oldGameId,
                    Arg.Any<string>(),
                    Arg.Any<string>());

            // Load another game into the same component instance.
            cut.Render(parameters => parameters
                .Add(page => page.GameId, newGameId));

            cut.WaitForAssertion(() =>
            {
                var row = cut.Find("[data-testid='machine-thunderbird-2']");
                Assert.Contains("Europe", row.TextContent);
                Assert.DoesNotContain("South Pacific", row.TextContent);
            });

            // Allow the previous game's request to complete.
            oldMoveCompletion.SetResult(
                new ThunderbirdMovementResult(
                    ThunderbirdMovementOutcome.Success,
                    oldMovedGame));

            await obsoleteMoveTask;

            cut.WaitForAssertion(() =>
            {
                var row = cut.Find("[data-testid='machine-thunderbird-2']");
                Assert.Contains("Europe", row.TextContent);
                Assert.DoesNotContain("South Pacific", row.TextContent);
            });
        }

        [Fact]
        public async Task ObsoleteDestinationResultDoesNotOverwriteCurrentMovementDialog()
        {
            // Arrange
            var oldGameId = Guid.NewGuid();
            var newGameId = Guid.NewGuid();

            var oldGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: oldGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-1",
                        locationCode: "south-pacific",
                        locationDisplayName: "South Pacific",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "scott", characterDisplayName: "Scott")
                        ])
                ]);

            var newGame = GameDashboardViewModelFactory.CreateDashboard(
                gameId: newGameId,
                machines: [
                    GameDashboardViewModelFactory.CreateMachine(
                        thunderbirdCode: "thunderbird-3",
                        locationCode: "the-sun",
                        locationDisplayName: "The Sun",
                        occupants: [
                            GameDashboardViewModelFactory.CreateOccupant(characterCode: "alan", characterDisplayName: "Alan")
                        ])
                ]);

            var oldDestinationsCompletion =
                new TaskCompletionSource<IReadOnlyList<SelectionOptionViewModel>>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            var context = new GameDashboardPageTestContext(this);
            context.GameService
                .GetGameAsync(oldGameId)
                .Returns(oldGame);
            context.GameService
                .GetGameAsync(newGameId)
                .Returns(newGame);
            context.MovementClientService
                .GetAccessibleLocationsAsync("thunderbird-1")
                .Returns(oldDestinationsCompletion.Task);
            context.MovementClientService
                .GetAccessibleLocationsAsync("thunderbird-3")
                .Returns(
                [
                    new("the-sun", "The Sun"),
                    new("mars", "Mars")
                ]);

            DialogJsInterop.Register(this);

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(page => page.GameId, oldGameId));

            // Start, but deliberately do not await, the old destination retrieval.
            var obsoleteDestinationTask = cut.WaitForElement("[data-testid='move-thunderbird-1']")
                .ClickAsync(new MouseEventArgs());

            // Ensure the old operation has reached the service boundary.
            await context.MovementClientService.Received(1)
                .GetAccessibleLocationsAsync("thunderbird-1");

            // Load another game into the same component instance.
            cut.Render(parameters => parameters
                .Add(page => page.GameId, newGameId));

            var currentDestinationTask = cut.Find("[data-testid='move-thunderbird-3']")
                .ClickAsync(new MouseEventArgs());

            await currentDestinationTask;

            var currentDialog = cut.FindComponent<MoveThunderbirdDialog>();

            Assert.Contains("Mars", currentDialog.Markup);

            // Allow the previous game's request to complete.
            oldDestinationsCompletion.SetResult(
            [
                new("south-pacific", "South Pacific"),
                new("europe", "Europe")
            ]);

            await obsoleteDestinationTask;

            cut.WaitForAssertion(() =>
            {
                var dialog = cut.FindComponent<MoveThunderbirdDialog>();

                var optionValues = dialog.FindAll("#move-destination option")
                    .Select(option => option.GetAttribute("value"))
                    .ToArray();

                Assert.Contains("mars", optionValues);
                Assert.DoesNotContain("europe", optionValues);
                Assert.DoesNotContain("south-pacific", optionValues);
            });
        }
    }
}
