using Bunit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
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

            var response = CreateDashboard(gameId);

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

            var response = CreateDashboard(gameId);

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
        public void Retry_WhenSecondRequestSucceeds_DisplaysGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var response = CreateDashboard(gameId);
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
        public void GameNotFound_DisplaysReturnHomeLink()
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

        private GameDashboardViewModel CreateDashboard(Guid gameId)
        {
            return new GameDashboardViewModel(
                GameId: gameId,
                ThunderbirdMachines: [
                    new ThunderbirdMachinesDashboardViewModel(
                        ThunderbirdCode: "tb1",
                        ThunderbirdDisplayName: "Thunderbird 1",
                        LocationCode: "loc1",
                        LocationDisplayName: "Location 1",
                        Occupants: [])
                    ]
                );
        }
    }
}
