using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public sealed class CreateNewGameHandler : IRequestHandler<CreateNewGameCommand, CreateNewGameResult>
    {
        private readonly StandardGameSetupFactory _gameFactory;
        private readonly IGameStateIntegrityValidator _gameStateIntegrityValidator;
        private readonly IGameRepository _gameRepository;
        private readonly ICharacterExistenceLookup _characterExistenceLookup;

        public CreateNewGameHandler(
            StandardGameSetupFactory standardGameSetupFactory,
            IGameStateIntegrityValidator gameStateIntegrityValidator,
            IGameRepository gameRepository,
            ICharacterExistenceLookup characterExistenceLookup)
        {
            _gameFactory = standardGameSetupFactory;
            _gameStateIntegrityValidator = gameStateIntegrityValidator;
            _gameRepository = gameRepository;
            _characterExistenceLookup = characterExistenceLookup;
        }

        public async Task<CreateNewGameResult> Handle(CreateNewGameCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.TurnOrder, nameof(request.TurnOrder));

            if (request.TurnOrder.Count < 2)
            {
                throw new InvalidTurnOrderException("A game must have at least 2 characters.");
            }

            if (request.TurnOrder.Count > 4)
            {
                throw new InvalidTurnOrderException($"A game can have a maximum of 4 characters, but received {request.TurnOrder.Count}.");
            }

            if (request.TurnOrder.Count != request.TurnOrder.Distinct().Count())
            {
                throw new InvalidTurnOrderException("The turn order contains duplicate characters.");
            }

            foreach (var character in request.TurnOrder)
            {
                if (!_characterExistenceLookup.Exists(character))
                {
                    throw CharacterNotFoundException.Create(character);
                }
            }

            var game = _gameFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, request.TurnOrder);

            _gameStateIntegrityValidator.Validate(game);

            await _gameRepository.CreateNewGameSession(game, cancellationToken);

            return new CreateNewGameResult(game);
        }
    }
}
