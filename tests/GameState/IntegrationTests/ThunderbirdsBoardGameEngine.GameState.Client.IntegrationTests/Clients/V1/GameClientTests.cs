using System.Net;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Client.IntegrationTests.Fixtures;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.TransferCharacters;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Client.IntegrationTests.Clients.V1
{
    [Collection("Game State API integration")]
    public class GameClientTests
    {
        private readonly GameStateApiIntegrationFixture _fixture;
        private readonly IGameClient _client;

        public GameClientTests(GameStateApiIntegrationFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task CreateNewGame()
        {
            // Arrange

            // Act
            var response = await _client.CreateGameAsync(CancellationToken.None);

            // Assert
            AssertResponse(response, HttpStatusCode.Created);

            var result = Assert.IsType<GameStateResponseDto>(response.Data);
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.GameId);

            var game = await _fixture.LoadGame(result.GameId, CancellationToken.None);
            Assert.NotNull(game);
        }

        [Fact]
        public async Task GetGameById()
        {
            // Arrange
            var seededGame = await _fixture.SeedGame(CancellationToken.None);

            // Act
            var response = await _client.GetGameStateAsync(seededGame.Id, CancellationToken.None);

            // Assert
            AssertResponse(response);
            _ = AssertGameResponseDto(response, seededGame.Id);
        }

        [Fact]
        public async Task GetGameByIdReturnsNotFoundWhenGameDoesNotExist()
        {
            // Arrange
            var nonExistentGameId = Guid.NewGuid();

            // Act
            var response = await _client.GetGameStateAsync(nonExistentGameId, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(response.ErrorMessage);
        }

        [Fact]
        public async Task MoveThunderbird()
        {
            // Arrange
            var seededGame = await _fixture.SeedGame(CancellationToken.None);
            var thunderbirdId = new ThunderbirdCode("fab-1");
            var destination = new LocationCode("south-pacific");

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = destination.Value
            };

            // Act
            var response = await _client.MoveThunderbirdMachineAsync(
                seededGame.Id,
                thunderbirdId.Value,
                request,
                CancellationToken.None);

            // Assert
            AssertResponse(response);

            var result = AssertGameResponseDto(response, seededGame.Id);
            Assert.Equal(result.ThunderbirdMachines.Single(m => m.ThunderbirdCode == thunderbirdId.Value).LocationCode, destination.Value);

            var updatedGame = await _fixture.LoadGame(seededGame.Id, CancellationToken.None);
            Assert.NotNull(updatedGame);

            var updatedThunderbird = updatedGame.Machines.Single(m => m.Key == thunderbirdId);
            Assert.Equal(destination, updatedThunderbird.Value);
        }

        [Fact]
        public async Task TransferCharacters()
        {
            // Arrange
            var seededGame = await _fixture.SeedGame(CancellationToken.None);

            var character = KnownCharacterCodes.Gordon;
            var thunderbird = KnownThunderbirdCodes.Thunderbird2;

            var request = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = character.Value,
                        DestinationThunderbirdCode = thunderbird.Value
                    }
                ]
            };

            // Act
            var response = await _client.TransferCharacters(seededGame.Id, request, CancellationToken.None);

            // Assert
            AssertResponse(response);

            var result = AssertGameResponseDto(response, seededGame.Id);
            Assert.Contains(character.Value, result.ThunderbirdMachines.Single(m => m.ThunderbirdCode == thunderbird.Value).OccupantCharacterCodes);

            var updatedGame = await _fixture.LoadGame(seededGame.Id, CancellationToken.None);
            Assert.NotNull(updatedGame);

            var updatedCharacter = updatedGame.Characters.Single(c => c.Key == character);
            Assert.Equal(thunderbird, updatedCharacter.Value);
        }

        private static void AssertResponse(ApiResult<GameStateResponseDto> response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Null(response.ErrorMessage);
        }

        private static GameStateResponseDto AssertGameResponseDto(ApiResult<GameStateResponseDto> response, Guid gameId)
        {
            var result = Assert.IsType<GameStateResponseDto>(response.Data);
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameId);
            return result;
        }
    }
}
