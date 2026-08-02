using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    internal sealed class MoveThunderbirdHandler : IRequestHandler<MoveThunderbirdCommand, MoveThunderbirdResult>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IValidateMovementGateway _movementGateway;
        private readonly IGameStateIntegrityValidator _gameStateIntegrityValidator;

        public MoveThunderbirdHandler(IGameRepository gameRepository, IValidateMovementGateway movementGateway, IGameStateIntegrityValidator gameStateIntegrityValidator)
        {
            _gameRepository = gameRepository;
            _movementGateway = movementGateway;
            _gameStateIntegrityValidator = gameStateIntegrityValidator;
        }

        public async Task<MoveThunderbirdResult> Handle(MoveThunderbirdCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetGameSessionById(request.GameId, cancellationToken)
                ?? throw new GameNotFoundException();

            _gameStateIntegrityValidator.Validate(game);

            var origin = game.GetThunderbirdMachineLocation(request.Thunderbird);

            var decision = await _movementGateway.ValidateMovement(request.Thunderbird, origin, request.Destination, cancellationToken);

            if (!decision.IsValid)
            {
                throw new ThunderbirdMovementRejectedException(decision.Messages);
            }

            game.MoveThunderbirdMachine(request.Thunderbird, request.Destination);

            _gameStateIntegrityValidator.Validate(game);

            await _gameRepository.UpdateGameSession(game, cancellationToken);

            return new MoveThunderbirdResult(game);
        }
    }
}
