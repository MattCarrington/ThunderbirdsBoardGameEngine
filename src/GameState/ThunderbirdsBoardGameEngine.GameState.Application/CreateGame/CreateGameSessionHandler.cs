using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public class CreateGameSessionHandler : IRequestHandler<CreateGameSessionCommand, CreateGameSessionResponse>
    {
        private readonly IGameSessionRespository _gameSessionRespository;

        public CreateGameSessionHandler(IGameSessionRespository gameSessionRespository)
        {
            _gameSessionRespository = gameSessionRespository;
        }

        public Task<CreateGameSessionResponse> Handle(CreateGameSessionCommand request, CancellationToken cancellationToken)
        {
            var gameId = Guid.NewGuid();

            var vehicleDefinitions = new Dictionary<ThunderbirdCode, LocationCode>
            {
                { new ThunderbirdCode("thunderbird-1"), new LocationCode("south-pacific") },
                { new ThunderbirdCode("thunderbird-2"), new LocationCode("south-pacific") },
                { new ThunderbirdCode("thunderbird-3"), new LocationCode("south-pacific") },
                { new ThunderbirdCode("thunderbird-4"), new LocationCode("south-pacific") },
                { new ThunderbirdCode("thunderbird-5"), new LocationCode("geo-stationary-orbit") },
                { new ThunderbirdCode("fab-1"), new LocationCode("europe") }
            };

            var gameSession = new GameSession(gameId, vehicleDefinitions);
            _gameSessionRespository.SaveGameSession(gameSession);

            return Task.FromResult(new CreateGameSessionResponse(gameSession));
        }
    }
}
