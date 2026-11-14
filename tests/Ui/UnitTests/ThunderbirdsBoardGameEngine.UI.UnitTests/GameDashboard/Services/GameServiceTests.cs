using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.TransferCharacters;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.GameDashboard.Services
{
    public class GameServiceTests
    {
        [Fact]
        public async Task CreateGameAsync_WhenCalled_CreatesGameSuccessfully()
        {
            // Arrange
            var dto = new GameStateResponseDto
            {
                GameId = Guid.NewGuid(),
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.Created);

            var client = Substitute.For<IGameClient>();
            client.CreateGameAsync(Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var game = await service.CreateGameAsync();

            // Assert
            Assert.False(game == Guid.Empty);

            await client.Received(1).CreateGameAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateGameAsync_WhenClientReturnsError_ThrowsInvalidOperationException()
        {
            // Arrange
            var apiResult = ApiResult<GameStateResponseDto>.Failure("Error", HttpStatusCode.InternalServerError);

            var client = Substitute.For<IGameClient>();
            client.CreateGameAsync(Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateGameAsync());
        }

        [Fact]
        public async Task CreateGameAsync_WhenClientReturnsNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(null, HttpStatusCode.Created);

            var client = Substitute.For<IGameClient>();
            client.CreateGameAsync(Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateGameAsync());
        }

        [Fact]
        public async Task GetGameAsync_WhenCalled_ReturnsGameSuccessfully()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.OK);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var game = await service.GetGameAsync(gameId);

            // Assert
            Assert.NotNull(game);
            Assert.Equal(gameId, game.GameId);

            await client.Received(1).GetGameStateAsync(gameId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetGameAsync_WhenClientReturnsNotFound_ReturnsNull()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Not Found", HttpStatusCode.NotFound);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var game = await service.GetGameAsync(gameId);

            // Assert
            Assert.Null(game);
        }

        [Fact]
        public async Task GetGameAsync_WhenClientReturnsError_ThrowsInvalidOperationException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Error", HttpStatusCode.InternalServerError);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetGameAsync(gameId));
        }

        [Fact]
        public async Task GetGameAsync_WhenClientReturnsNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(null, HttpStatusCode.OK);

            var client = Substitute.For<IGameClient>();
            client.GetGameStateAsync(gameId, Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetGameAsync(gameId));
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenCalled_ReturnsSuccessResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.OK);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode);

            // Assert
            Assert.Equal(ThunderbirdMovementOutcome.Success, result.Outcome);
            await client.Received(1).MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenClientReturnsUnprocessableEntity_ReturnsRejectedResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Unprocessable Entity", HttpStatusCode.UnprocessableEntity);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode);

            // Assert
            Assert.Equal(ThunderbirdMovementOutcome.Rejected, result.Outcome);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenClientReturnsNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Not Found", HttpStatusCode.NotFound);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode);

            // Assert
            Assert.Equal(ThunderbirdMovementOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task MoveThunderbirdAsync_WhenClientReturnsError_ThrowsInvalidOperationException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var destinationLocationCode = "LOC1";

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Error", HttpStatusCode.InternalServerError);

            var client = Substitute.For<IGameClient>();
            client.MoveThunderbirdMachineAsync(gameId, thunderbirdCode, Arg.Any<MoveThunderbirdMachineRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task MoveThunderbirdAsync_WhenDestinationLocationCodeIsNullOrWhitespace_ThrowsArgumentException(string? destinationLocationCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = "TB1";

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.MoveThunderbirdAsync(gameId, thunderbirdCode, destinationLocationCode));
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenValidSingleTransfer_BuildsSingleAssignmentRequest()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: null
            );

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.OK);

            TransferCharactersRequestDto? capturedRequest = null;

            var client = Substitute.For<IGameClient>();
            client
                .TransferCharactersAsync(
                    gameId,
                    Arg.Do<TransferCharactersRequestDto>(
                        request => capturedRequest = request),
                    Arg.Any<CancellationToken>())
                .Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            await service.TransferCharactersAsync(gameId, selection);

            // Assert
            Assert.NotNull(capturedRequest);

            var assignment = Assert.Single(capturedRequest.Assignments);
            Assert.Equal(selection.CharacterCode, assignment.CharacterCode);
            Assert.Equal(selection.DestinationThunderbirdCode, assignment.DestinationThunderbirdCode);
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenValidReciprocalSwap_BuildsTwoAssignments()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: "CHAR2"
            );

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.OK);

            TransferCharactersRequestDto? capturedRequest = null;

            var client = Substitute.For<IGameClient>();
            client
                .TransferCharactersAsync(
                    gameId,
                    Arg.Do<TransferCharactersRequestDto>(
                        request => capturedRequest = request),
                    Arg.Any<CancellationToken>())
                .Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            await service.TransferCharactersAsync(gameId, selection);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(2, capturedRequest.Assignments.Count);

            var assignment1 = Assert.Single(capturedRequest.Assignments, a => a.CharacterCode == selection.CharacterCode);
            Assert.Equal(selection.DestinationThunderbirdCode, assignment1.DestinationThunderbirdCode);
            Assert.Equal(selection.CharacterCode, assignment1.CharacterCode);

            var assignment2 = Assert.Single(capturedRequest.Assignments, a => a.CharacterCode == selection.ReturningCharacterCode);
            Assert.Equal(selection.SourceThunderbirdCode, assignment2.DestinationThunderbirdCode);
            Assert.Equal(selection.ReturningCharacterCode, assignment2.CharacterCode);
        }

        [Fact]
        public async Task TransferCharactersAsync_SuccessfulResponse_ReturnsMappedGameState()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: "CHAR2"
            );

            var dto = new GameStateResponseDto
            {
                GameId = gameId,
                ThunderbirdMachines = []
            };

            var apiResult = ApiResult<GameStateResponseDto>.SuccessResult(dto, HttpStatusCode.OK);

            var client = Substitute.For<IGameClient>();
            client.TransferCharactersAsync(gameId, Arg.Any<TransferCharactersRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.TransferCharactersAsync(gameId, selection);

            // Assert
            Assert.Equal(CharacterTransferOutcome.Success, result.Outcome);
            Assert.NotNull(result.UpdatedGameState);
            Assert.Equal(gameId, result.UpdatedGameState.GameId);

            await client.Received(1).TransferCharactersAsync(gameId, Arg.Any<TransferCharactersRequestDto>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenClientReturnsUnprocessableEntity_ReturnsRejectedResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: null
            );

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Unprocessable Entity", HttpStatusCode.UnprocessableEntity);

            var client = Substitute.For<IGameClient>();
            client.TransferCharactersAsync(gameId, Arg.Any<TransferCharactersRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.TransferCharactersAsync(gameId, selection);

            // Assert
            Assert.Equal(CharacterTransferOutcome.Rejected, result.Outcome);
            Assert.Null(result.UpdatedGameState);
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenClientReturnsNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: null
            );

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Not Found", HttpStatusCode.NotFound);

            var client = Substitute.For<IGameClient>();
            client.TransferCharactersAsync(gameId, Arg.Any<TransferCharactersRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act
            var result = await service.TransferCharactersAsync(gameId, selection);

            // Assert
            Assert.Equal(CharacterTransferOutcome.NotFound, result.Outcome);
            Assert.Null(result.UpdatedGameState);
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenClientReturnsError_ThrowsInvalidOperationException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: null
            );

            var apiResult = ApiResult<GameStateResponseDto>.Failure("Error", HttpStatusCode.InternalServerError);

            var client = Substitute.For<IGameClient>();
            client.TransferCharactersAsync(gameId, Arg.Any<TransferCharactersRequestDto>(), Arg.Any<CancellationToken>()).Returns(apiResult);

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.TransferCharactersAsync(gameId, selection));
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenSelectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.TransferCharactersAsync(gameId, null!));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task TransferCharactersAsync_WhenCharacterCodeIsNullOrWhitespace_ThrowsArgumentException(string? characterCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: characterCode,
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: null
            );

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.TransferCharactersAsync(gameId, selection));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task TransferCharactersAsync_WhenDestinationThunderbirdCodeIsNullOrWhitespace_ThrowsArgumentException(string? destinationThunderbirdCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: destinationThunderbirdCode,
                ReturningCharacterCode: null
            );

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.TransferCharactersAsync(gameId, selection));
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenSourceAndDestinationThunderbirdCodesAreSame_ThrowsArgumentException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "thunderbirdCode",
                DestinationThunderbirdCode: "thunderbirdCode",
                ReturningCharacterCode: null
            );

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.TransferCharactersAsync(gameId, selection));
        }

        [Fact]
        public async Task TransferCharactersAsync_WhenReturningCharacterCodeIsSameAsCharacterCode_ThrowsArgumentException()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: "CHAR1"
            );

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.TransferCharactersAsync(gameId, selection));
        }

        [Theory]
        [ClassData(typeof(WhiteSpaceStringData))]
        public async Task TransferCharactersAsync_WhenReturningCharacterCodeIsWhitespace_ThrowsArgumentException(string returningCharacterCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: "TB1",
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: returningCharacterCode
            );

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.TransferCharactersAsync(gameId, selection));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task TransferCharactersAsync_WhenSourceThunderbirdCodeIsNullOrWhitespace_ThrowsArgumentException(string? sourceThunderbirdCode)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var selection = new CharacterTransferSelection
            (
                CharacterCode: "CHAR1",
                SourceThunderbirdCode: sourceThunderbirdCode,
                DestinationThunderbirdCode: "TB2",
                ReturningCharacterCode: "CHAR2"
            );

            var client = Substitute.For<IGameClient>();

            var service = CreateGameService(client);

            // Act & Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.TransferCharactersAsync(gameId, selection));
        }

        private static GameService CreateGameService(IGameClient client)
        {
            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();

            var mapper = new GameDashboardMapper(thunderbirdCatalog, locationCatalog, characterCatalog);

            return new GameService(client, mapper);
        }
    }
}
