using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.WireMock;
using ThunderbirdsBoardGameEngine.GameState.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.TestUtils.WireMock.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Pages;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests.Pages
{
    [Collection("WireMock")]
    public class HomePageTests : BunitContext
    {
        private readonly GameStub _gameStub;

        public HomePageTests(WireMockFixture wireMockFixture)
        {
            var host = wireMockFixture.Host;
            host.Reset();

            _gameStub = host.GameStub();
            _gameStub.RegisterIncorrectHeaderGuard();
            _gameStub.RegisterMissingHeaderGuard();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "GameStateClient:BaseAddress", host.Url }
                })
                .Build();

            Services.AddReferenceData();
            Services.AddGameStateClients(configuration);
            Services.AddUiServices();
        }

        [Fact]
        public void NavigatesToGameDashboardOnSuccessfulCreate()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            _gameStub.RegisterCreateGameSuccess(dto);

            var navigation = Services.GetRequiredService<NavigationManager>();

            var cut = Render<Home>();

            // Act
            cut.Find("[data-testid='create-game']").ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
                Assert.EndsWith(
                    $"/games/{gameId:D}",
                    navigation.Uri));
        }

        [Fact]
        public void StartingGameShouldDisplayErrorWhenApiFails()
        {
            // Arrange
            _gameStub.RegisterCreateGameError(message: "The game could not be created. Please try again.");

            var cut = Render<Home>();

            // Act
            cut.Find("[data-testid='create-game']").ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Equal(
                    "The game could not be created. Please try again.",
                    cut.Find("[role='alert']").TextContent.Trim());
            });
        }
    }
}
