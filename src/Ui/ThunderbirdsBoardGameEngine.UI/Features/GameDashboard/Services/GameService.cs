using System.Net;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.EndTurn;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.TransferCharacters;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services
{
    public class GameService : IGameService
    {
        private readonly IGameClient _client;
        private readonly GameDashboardMapper _mapper;

        public GameService(IGameClient client, GameDashboardMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<Guid> CreateGameAsync(IReadOnlyList<string> turnOrder)
        {
            ArgumentNullException.ThrowIfNull(turnOrder);

            if (turnOrder.Count is < 2 or > 4)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(turnOrder),
                    turnOrder.Count,
                    "Turn order must contain between two and four characters.");
            }

            if (turnOrder.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException(
                    "Turn order cannot contain null or whitespace values.",
                    nameof(turnOrder));
            }

            if (turnOrder.Distinct(StringComparer.Ordinal).Count() != turnOrder.Count)
            {
                throw new ArgumentException(
                    "Turn order cannot contain duplicate characters.",
                    nameof(turnOrder));
            }

            var request = new CreateGameRequestDto
            {
                TurnOrder = turnOrder.ToList()
            };

            var result = await _client.CreateGameAsync(request);

            if (!result.Success || result.Data is null)
            {
                throw new InvalidOperationException("The game could not be created.");
            }

            return result.Data.GameId;
        }

        public async Task<GameDashboardViewModel?> GetGameAsync(Guid gameId)
        {
            var result = await _client.GetGameStateAsync(gameId);

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!result.Success || result.Data is null)
            {
                throw new InvalidOperationException("The game could not be retrieved.");
            }

            return _mapper.ToViewModel(result.Data);
        }

        public async Task<ThunderbirdMovementResult> MoveThunderbirdAsync(Guid gameId, string thunderbirdMachineCode, string destinationLocationCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(destinationLocationCode, nameof(destinationLocationCode));

            var request = new MoveThunderbirdMachineRequestDto
            {
                Destination = destinationLocationCode
            };

            var result = await _client.MoveThunderbirdMachineAsync(gameId, thunderbirdMachineCode, request);

            if (result.Success && result.Data is not null)
            {
                return new ThunderbirdMovementResult(
                    Outcome: ThunderbirdMovementOutcome.Success,
                    UpdatedGameState: _mapper.ToViewModel(result.Data));
            }

            if (result.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                return new ThunderbirdMovementResult(
                    Outcome: ThunderbirdMovementOutcome.Rejected);
            }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return new ThunderbirdMovementResult(
                    Outcome: ThunderbirdMovementOutcome.NotFound);
            }

            throw new InvalidOperationException(
                "The Thunderbird Machine could not be moved.");
        }

        public async Task<CharacterTransferResult> TransferCharactersAsync(Guid gameId, CharacterTransferSelection selection)
        {
            ArgumentNullException.ThrowIfNull(selection, nameof(selection));
            ArgumentException.ThrowIfNullOrWhiteSpace(selection.CharacterCode, nameof(selection.CharacterCode));
            ArgumentException.ThrowIfNullOrWhiteSpace(selection.DestinationThunderbirdCode, nameof(selection.DestinationThunderbirdCode));
            ArgumentException.ThrowIfNullOrWhiteSpace(selection.SourceThunderbirdCode, nameof(selection.SourceThunderbirdCode));

            if (selection.SourceThunderbirdCode == selection.DestinationThunderbirdCode)
            {
                throw new ArgumentException(
                    "Source and destination Thunderbird Machines must differ.",
                    nameof(selection));
            }

            var assignments = new List<CharacterTransferAssignmentDto>
            {
                new()
                {
                    CharacterCode = selection.CharacterCode,
                    DestinationThunderbirdCode = selection.DestinationThunderbirdCode
                }
            };

            if (selection.ReturningCharacterCode is not null)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(selection.ReturningCharacterCode, nameof(selection.ReturningCharacterCode));

                if (selection.ReturningCharacterCode == selection.CharacterCode)
                {
                    throw new ArgumentException(
                        "The returning character must differ from the character being transferred.",
                        nameof(selection));
                }

                assignments.Add(new CharacterTransferAssignmentDto
                {
                    CharacterCode = selection.ReturningCharacterCode,
                    DestinationThunderbirdCode = selection.SourceThunderbirdCode
                });
            }

            var request = new TransferCharactersRequestDto
            {
                Assignments = assignments
            };

            var result = await _client.TransferCharactersAsync(gameId, request);

            if (result.Success && result.Data is not null)
            {
                return new CharacterTransferResult(
                    Outcome: CharacterTransferOutcome.Success,
                    UpdatedGameState: _mapper.ToViewModel(result.Data));
            }

            if (result.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                return new CharacterTransferResult(
                    Outcome: CharacterTransferOutcome.Rejected);
            }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return new CharacterTransferResult(
                    Outcome: CharacterTransferOutcome.NotFound);
            }

            throw new InvalidOperationException(
                "The character could not be transferred.");
        }

        public async Task<EndTurnResult> EndTurnAsync(Guid gameId, string activeCharacterCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(activeCharacterCode, nameof(activeCharacterCode));

            var request = new EndTurnRequestDto
            {
                ExpectedActiveCharacterCode = activeCharacterCode
            };

            var result = await _client.EndTurnAsync(gameId, request);

            if (result.Success && result.Data is not null)
            {
                return new EndTurnResult(
                    Outcome: EndTurnOutcome.Success,
                    UpdatedGameState: _mapper.ToViewModel(result.Data));
            }

            if (result.StatusCode == HttpStatusCode.Conflict)
            {
                return new EndTurnResult(
                    Outcome: EndTurnOutcome.Conflict);
            }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return new EndTurnResult(
                    Outcome: EndTurnOutcome.NotFound);
            }

            throw new InvalidOperationException("The turn could not be ended.");
        }
    }
}
