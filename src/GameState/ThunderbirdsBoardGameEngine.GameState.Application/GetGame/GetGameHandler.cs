using MediatR;

namespace ThunderbirdsBoardGameEngine.GameState.Application.GetGame
{
    public sealed class GetGameHandler : IRequestHandler<GetGameQuery, GetGameResponse>
    {
        private readonly IGameRepository _gameRepository;

        public GetGameHandler(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GetGameResponse> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetGameSessionById(request.GameId, cancellationToken)
                ?? throw new KeyNotFoundException($"Game with ID {request.GameId} not found."); // TODO: Replace with a more specific exception type if needed.

            return new GetGameResponse(game);
        }
    }
}
