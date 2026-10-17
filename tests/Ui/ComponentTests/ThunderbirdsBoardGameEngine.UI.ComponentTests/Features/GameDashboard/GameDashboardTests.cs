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
        public void ClickingMoveShouldRequestMovementForSelectedMachine()
        {
            // Arrange
            ThunderbirdMachinesDashboardViewModel? selectedMachine = null;

            var machines = new List<ThunderbirdMachinesDashboardViewModel>
            {
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-1"),
                GameDashboardViewModelFactory.CreateMachine(thunderbirdCode: "thunderbird-2")
            };

            var cut = Render<ThunderbirdMachinesSegment>(parameters => parameters
                .Add(p => p.Machines, machines)
                .Add(p => p.MoveRequested, machine => selectedMachine = machine));

            // Act
            cut.Find("[data-testid='move-thunderbird-1']").Click();

            // Assert
            Assert.NotNull(selectedMachine);
            Assert.Equal("thunderbird-1", selectedMachine.ThunderbirdCode);
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

            await context.GameService.Received(0)
                .MoveThunderbirdAsync(
                    gameId,
                    "thunderbird-1",
                    "europe");
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
    }
}
