using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.Application.EndTurn
{
    internal class EndTurnHandler : IRequestHandler<EndTurnCommand, EndTurnResult>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateIntegrityValidator _gameStateIntegrityValidator;

        public EndTurnHandler(IGameRepository gameRepository, IGameStateIntegrityValidator gameStateIntegrityValidator)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _gameStateIntegrityValidator = gameStateIntegrityValidator;
        }

        public async Task<EndTurnResult> Handle(EndTurnCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetGameSessionById(request.GameId, cancellationToken)
                ?? throw new GameNotFoundException();

            _gameStateIntegrityValidator.Validate(game);

            game.AdvanceTurn();

            _gameStateIntegrityValidator.Validate(game);

            await _gameRepository.UpdateGameSession(game, cancellationToken);

            return new EndTurnResult(game);
        }
    }

    public record EndTurnResult(Game GameSession);

    public record EndTurnCommand(Guid GameId) : IRequest<EndTurnResult>;
}
