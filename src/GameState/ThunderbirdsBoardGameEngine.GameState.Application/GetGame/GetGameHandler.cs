using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.GameState.Application.GetGame
{
    public sealed class GetGameHandler : IRequestHandler<GetGameQuery, GetGameResponse>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateIntegrityValidator _integrityValidator;

        public GetGameHandler(IGameRepository gameRepository, IGameStateIntegrityValidator integrityValidator)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _integrityValidator = integrityValidator ?? throw new ArgumentNullException(nameof(integrityValidator));
        }

        public async Task<GetGameResponse> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetGameSessionById(request.GameId, cancellationToken)
                ?? throw new GameNotFoundException();

            _integrityValidator.Validate(game);

            return new GetGameResponse(game);
        }
    }
}
