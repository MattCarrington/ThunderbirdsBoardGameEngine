using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.WireMock;
using ThunderbirdsBoardGameEngine.GameState.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock;
using ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils.WireMock.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests.Pages
{
    [Collection("WireMock")]
    public class GameDashboardPageTests : BunitContext
    {
        private readonly GameStub _gameStub;
        private readonly MovementStub _movementStub;

        public GameDashboardPageTests(WireMockFixture wireMockFixture)
        {
            Register(this);

            var host = wireMockFixture.Host;
            host.Reset();

            _gameStub = host.GameStub();
            _gameStub.RegisterIncorrectHeaderGuard();
            _gameStub.RegisterMissingHeaderGuard();

            _movementStub = host.MovementStub();
            _movementStub.RegisterIncorrectHeaderGuard();
            _movementStub.RegisterMissingHeaderGuard();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "GameStateClient:BaseAddress", host.Url },
                    { "RulesClient:BaseAddress", host.Url }
                })
                .Build();

            Services.AddReferenceData();
            Services.AddGameStateClients(configuration);
            Services.AddRulesClients(configuration);
            Services.AddUiServices();
        }

        [Fact]
        public void LoadsGameStateOnPageLoad()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines =
                [
                    new()
                    {
                        ThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value,
                        LocationCode = KnownLocationCodes.SouthPacific.Value,
                        OccupantCharacterCodes = [
                            KnownCharacterCodes.Scott.Value,
                            KnownCharacterCodes.Virgil.Value
                        ]
                    },
                    new()
                    {
                        ThunderbirdCode = KnownThunderbirdCodes.Thunderbird2.Value,
                        LocationCode = KnownLocationCodes.SouthPacific.Value,
                        OccupantCharacterCodes = [
                            KnownCharacterCodes.John.Value
                        ]
                    }
                ]
            };

            _gameStub.RegisterGetGameStateSuccess(dto);

            // Act
            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Assert
            cut.WaitForAssertion(() =>
            {
                cut.WaitForAssertion(() =>
                {
                    Assert.Contains("Thunderbird 1", cut.Markup);
                    Assert.Contains("South Pacific", cut.Markup);
                    Assert.Contains("Scott", cut.Markup);

                    Assert.Contains("Thunderbird 2", cut.Markup);
                    Assert.Contains("Virgil", cut.Markup);
                    Assert.Contains("John", cut.Markup);
                });
            });
        }

        [Fact]
        public void ShowsErrorMessageOnFailedLoad()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            _gameStub.RegisterGetGameStateNotFound();

            // Act
            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Assert
            var notFound = cut.WaitForElement("[data-testid='game-not-found']");

            Assert.Equal("This game could not be found.", notFound.TextContent.Trim());

            Assert.Empty(cut.FindAll("[data-testid='loading-game']"));
        }

        [Fact]
        public async Task RefreshesDashboardOnSuccessfulMoveAsync()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var initialDto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines =
                [
                    new()
                    {
                        ThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value,
                        LocationCode = KnownLocationCodes.SouthPacific.Value,
                        OccupantCharacterCodes = [
                            KnownCharacterCodes.Scott.Value
                        ]
                    },
                    new()                     {
                        ThunderbirdCode = KnownThunderbirdCodes.Thunderbird2.Value,
                        LocationCode = KnownLocationCodes.Europe.Value,
                        OccupantCharacterCodes = [
                            KnownCharacterCodes.Virgil.Value
                        ]
                    }
                ]
            };

            var moveDto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines =
                [
                    new()
                    {
                        ThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value,
                        LocationCode = "north-atlantic",
                        OccupantCharacterCodes = [
                            KnownCharacterCodes.Scott.Value
                        ]
                    },
                    new()                     {
                        ThunderbirdCode = KnownThunderbirdCodes.Thunderbird2.Value,
                        LocationCode = KnownLocationCodes.Europe.Value,
                        OccupantCharacterCodes = [
                            KnownCharacterCodes.Virgil.Value
                        ]
                    }
                ]
            };

            var locations = new[] { "europe", "north-atlantic", "south-pacific" };

            _gameStub.RegisterGetGameStateSuccess(initialDto);
            _gameStub.RegisterMoveThunderbirdMachineSuccess(moveDto);

            _movementStub.RegisterGetAccessibleLocationsSuccess(new AccessibleLocationsResponseDto
            {
                AccessibleLocations = locations
            });

            var cut = Render<GameDashboardPage>(parameters => parameters
                .Add(p => p.GameId, gameId));

            // Act     
            var moveButton = cut.WaitForElement("[data-testid='move-thunderbird-1']");

            await moveButton.ClickAsync(new MouseEventArgs());

            cut.WaitForState(() =>
                cut.FindComponents<MoveThunderbirdDialog>().Count == 1);

            var dialog = cut.FindComponent<MoveThunderbirdDialog>();

            dialog.Find("#move-destination").Change("north-atlantic");

            await dialog
                .Find("[data-testid='confirm-move']")
                .ClickAsync(new MouseEventArgs());

            // Assert - The dashboard should refresh and show the new location
            cut.WaitForAssertion(() =>
            {
                Assert.Empty(cut.FindComponents<MoveThunderbirdDialog>());
                Assert.Contains("North Atlantic", cut.Markup);
                Assert.DoesNotContain("South Pacific", cut.Markup);
            });
        }

        private static void Register(BunitContext context)
        {
            var module = context.JSInterop.SetupModule("./js/dialog.js");

            module.SetupVoid("show", _ => true)
                .SetVoidResult();

            module.SetupVoid("close", _ => true)
                .SetVoidResult();
        }
    }
}
