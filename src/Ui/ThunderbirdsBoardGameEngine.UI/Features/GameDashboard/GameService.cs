using ThunderbirdsBoardGameEngine.GameState.Client.Clients;
using ThunderbirdsBoardGameEngine.GameState.Contracts.V1;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public class GameService : IGameService
    {
        private IGameClient _gameClient;

        public GameService(IGameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public async Task<GameSessionViewModel?> CreateGameSessionViewModel(CancellationToken cancellationToken = default)
        {
            var result = await _gameClient.CreateGameStateAsync(cancellationToken);

            if (result.Success)
            {
                var gameSession = result.Data ?? throw new InvalidOperationException("Game session data is null.");
                var thunderbirdLocations = gameSession.ThunderbirdLocation.Select(tl => new ThunderbirdLocationViewModel(
                    tl.ThunderbirdCode,
                    tl.ThunderbirdCode,
                    tl.LocationCode,
                    tl.LocationCode
                )).ToList();

                return new GameSessionViewModel(gameSession.GameId, thunderbirdLocations);
            }

            // Handle error case as needed
            return null;
        }

        public async Task<GameSessionViewModel?> GetGameSessionViewModel(Guid gameId, CancellationToken cancellationToken = default)
        {
            var result = await _gameClient.GetGameStateAsync(gameId, cancellationToken);
            if (result.Success)
            {
                var gameSession = result.Data ?? throw new InvalidOperationException("Game session data is null.");
                var thunderbirdLocations = gameSession.ThunderbirdLocation.Select(tl => new ThunderbirdLocationViewModel(
                    tl.ThunderbirdCode,
                    tl.ThunderbirdCode,
                    tl.LocationCode,
                    tl.LocationCode
                )).ToList();
                return new GameSessionViewModel(gameSession.GameId, thunderbirdLocations);
            }
            // Handle error case as needed
            return null;
        }

        public async Task<GameSessionViewModel?> MoveThunderbirdAsync(Guid gameId, string thunderbirdCode, string destinationLocationCode, CancellationToken cancellationToken = default)
        {
            var request = new MoveThunderbirdLocationRequestDto
            {
                ThunderbirdCode = thunderbirdCode,
                Destination = destinationLocationCode
            };

            var result = await _gameClient.MoveThunderbirdAsync(gameId, request, cancellationToken);

            if (result.Success)
            {
                var gameSession = result.Data ?? throw new InvalidOperationException("Game session data is null.");
                var thunderbirdLocations = gameSession.ThunderbirdLocation.Select(tl => new ThunderbirdLocationViewModel(
                    tl.ThunderbirdCode,
                    tl.ThunderbirdCode,
                    tl.LocationCode,
                    tl.LocationCode
                )).ToList();

                return new GameSessionViewModel(gameSession.GameId, thunderbirdLocations);
            }
            // Handle error case as needed
            return null;
        }
    }
}
