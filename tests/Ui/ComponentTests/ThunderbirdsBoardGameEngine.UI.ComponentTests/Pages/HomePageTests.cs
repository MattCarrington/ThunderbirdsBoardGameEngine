using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Pages;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Pages
{
    public class HomePageTests : BunitContext
    {
        [Fact]
        public void CreateGameShouldNavigateToNewGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var context = new HomePageTestContext(this);
            context.GameService.CreateGameAsync().Returns(gameId);

            var navigation = Services.GetRequiredService<NavigationManager>();

            // Act
            var cut = Render<Home>();

            cut.Find("[data-testid='create-game']").Click();

            // Assert
            Assert.EndsWith($"/games/{gameId:D}", navigation.Uri);
        }

        [Fact]
        public void CreateGameShouldDisplayErrorWhenServiceThrowsException()
        {
            // Arrange
            var context = new HomePageTestContext(this);
            context.GameService.CreateGameAsync().ThrowsAsync(new Exception("Test exception"));

            // Act
            var cut = Render<Home>();

            cut.Find("[data-testid='create-game']").Click();

            // Assert
            Assert.Contains("The game could not be created. Please try again.", cut.Markup);
        }

        [Fact]
        public void ResumeGameShouldNavigateWhenIdentifierIsValid()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var context = new HomePageTestContext(this);
            context.GameService.CreateGameAsync().Returns(gameId);

            var navigation = Services.GetRequiredService<NavigationManager>();

            // Act
            var cut = Render<Home>();

            cut.Find("#game-identifier").Change(gameId.ToString());
            cut.Find("[data-testid='resume-game']").Click();

            // Assert
            Assert.EndsWith($"/games/{gameId:D}", navigation.Uri);
        }

        [Fact]
        public void ResumeGameShouldDisplayErrorWhenIdentifierIsInvalid()
        {
            // Arrange
            var context = new HomePageTestContext(this);

            // Act
            var cut = Render<Home>();

            cut.Find("#game-identifier").Change("invalid-guid");

            cut.Find("[data-testid='resume-game']").Click();

            // Assert
            Assert.Contains("Enter a valid game identifier.", cut.Markup);
        }

        [Fact]
        public async Task StartGameShouldBeDisabledWhileCreationIsPending()
        {
            var pendingCreation = new TaskCompletionSource<Guid>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            var context = new HomePageTestContext(this);
            context.GameService.CreateGameAsync().Returns(pendingCreation.Task);

            var component = Render<Home>();
            var button = component.Find("[data-testid='create-game']");

            var clickTask = button.ClickAsync(new MouseEventArgs());

            component.WaitForAssertion(() =>
            {
                Assert.True(button.HasAttribute("disabled"));
                Assert.Contains("Creating game", button.TextContent);
            });

            pendingCreation.SetResult(Guid.NewGuid());
            await clickTask;

            component.WaitForAssertion(() =>
                Assert.False(button.HasAttribute("disabled")));
        }
    }
}
