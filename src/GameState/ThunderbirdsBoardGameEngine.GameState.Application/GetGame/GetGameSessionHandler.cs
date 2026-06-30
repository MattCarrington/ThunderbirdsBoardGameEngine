using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;

namespace ThunderbirdsBoardGameEngine.GameState.Application.GetGame
{
    public class GetGameSessionHandler : IRequestHandler<GetGameSessionQuery, GetGameSessionResponse>
    {
        private readonly IGameSessionRespository _gameSessionRespository;

        public GetGameSessionHandler(IGameSessionRespository gameSessionRespository)
        {
            _gameSessionRespository = gameSessionRespository;
        }

        public async Task<GetGameSessionResponse> Handle(GetGameSessionQuery request, CancellationToken cancellationToken)
        {
            var gameSession = await _gameSessionRespository.GetGameSession(request.GameId, cancellationToken);
            return new GetGameSessionResponse(gameSession);
        }
    }
}
